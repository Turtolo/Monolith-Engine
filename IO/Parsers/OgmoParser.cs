using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Monolith.Graphics;
using Monolith.Managers;
using Monolith.Nodes;
using Monolith.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Monolith.IO
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OgmoAttribute : Attribute
    {
        public string Key { get; }
        public OgmoAttribute(string key) => Key = key;
    }

    public static class OgmoParser
    {
        private static OgmoFileInfo.Root LoadJson(string filename)
        {
            string json = File.ReadAllText(filename);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<OgmoFileInfo.Root>(json, options)!;
        }

        private static Dictionary<string, object> ParseValues(Dictionary<string, JsonElement> values)
        {
            return values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ValueKind switch
                {
                    JsonValueKind.String => (object)kvp.Value.GetString()!,
                    JsonValueKind.Number => kvp.Value.TryGetInt64(out long l) ? l : kvp.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null!,
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object>>(kvp.Value.GetRawText())!,
                    JsonValueKind.Array => JsonSerializer.Deserialize<List<object>>(kvp.Value.GetRawText())!,
                    _ => kvp.Value.GetRawText()
                }
            );
        }

        public static void SearchForDecals(string filename)
        {
            var root = LoadJson(filename);

            foreach (var layer in root.layers.Where(l => !string.IsNullOrEmpty(l.folder) && l.decals != null))
            {
                foreach (var decal in layer.decals)
                {
                    string textureName = Path.GetFileNameWithoutExtension(decal.texture);
                    string texturePathWithoutExtension = Path.Combine(
                        Path.GetDirectoryName(decal.texture) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(decal.texture)
                    );

                    MTexture texture = new(texturePathWithoutExtension);

                    Sprite2D sprite2D = new Sprite2D(new SpriteConfig
                    {
                        Parent = null,
                        Name = $"Decal: {texturePathWithoutExtension}",
                        Position = new Vector2(decal.x, decal.y),
                        Texture = new MTexture(texturePathWithoutExtension)
                    });
                }
            }
        }

        /// <summary>
        /// Searches through Ogmo's entites and assigns values to nodes.
        /// </summary>
        public static void SearchForNodes(string filename)
        {
            var root = LoadJson(filename);

            foreach (var entity in GetEntities(root))
            {
                var nodeType = FindNodeType(entity.name);
                if (nodeType == null)
                {
                    Console.WriteLine($"Type '{entity.name}' not found or not a subclass of Node2D.");
                    continue;
                }

                var config = CreateConfigInstance(nodeType, entity);

                Node2D node = (Node2D)Activator.CreateInstance(nodeType, config)!;
                CreateSubNodes(entity, nodeType, config, node);
            }
        }

        private static IEnumerable<dynamic> GetEntities(dynamic root)
        {
            var layers = ((IEnumerable<object>)root.layers)
                .Cast<dynamic>();

            return layers
                .Where(l => l.entities != null)
                .SelectMany(l => ((IEnumerable<object>)l.entities).Cast<dynamic>());
        }

        private static Type? FindNodeType(string entityName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .FirstOrDefault(t => t.IsSubclassOf(typeof(Node2D)) &&
                                    t.Name == entityName);
        }

        private static object CreateConfigInstance(Type nodeType, dynamic entity)
        {
            var ctor = nodeType.GetConstructors().First();
            var configType = ctor.GetParameters().First().ParameterType;

            var config = Activator.CreateInstance(configType)!;

            configType.GetProperty("Parent")?.SetValue(config, null);
            configType.GetProperty("Name")?.SetValue(config, entity.name);
            configType.GetProperty("Region")?.SetValue(config,
                new RectangleShape2D(entity.x, entity.y, entity.width, entity.height));

            Dictionary<string, object> dict =
                entity.values != null ? ParseValues(entity.values)
                                    : new Dictionary<string, object>();

            ApplyProperties(configType, config, dict, entity);

            return config;
        }

        private static void ApplyProperties(Type configType, object config,
                                            Dictionary<string, object> dict, dynamic entity)
        {
            foreach (var prop in configType.GetProperties().Where(p => p.CanWrite))
            {
                var attr = prop.GetCustomAttribute<OgmoAttribute>();
                var key = attr?.Key ?? prop.Name;

                if (typeof(Node2D).IsAssignableFrom(prop.PropertyType))
                {
                    var nested = CreateNestedNode(prop.PropertyType, entity);
                    prop.SetValue(config, nested);
                    continue;
                }

                if (dict.TryGetValue(key, out var raw))
                {
                    object val = Convert.ChangeType(
                        raw,
                        Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                    prop.SetValue(config, val);
                }
            }
        }

        private static Node2D? CreateNestedNode(Type nestedType, dynamic entity)
        {
            var ctor = nestedType.GetConstructors().First();
            var configType = ctor.GetParameters().First().ParameterType;

            var nestedConfig = Activator.CreateInstance(configType);
            if (nestedConfig == null) return null;

            configType.GetProperty("Name")?.SetValue(nestedConfig, entity.name + "_Nested");
            configType.GetProperty("Region")?.SetValue(nestedConfig,
                new RectangleShape2D(entity.x, entity.y, entity.width, entity.height));

            return (Node2D?)Activator.CreateInstance(nestedType, nestedConfig);
        }

        private static void CreateSubNodes(dynamic entity, Type nodeType, object baseConfig, Node2D parent)
        {
            if (entity.nodes == null)
                return;

            var configType = baseConfig.GetType();

            foreach (var n in entity.nodes)
            {
                configType.GetProperty("Region")?.SetValue(baseConfig,
                    new RectangleShape2D(n.x, n.y, entity.width, entity.height));

                Node2D subNode = (Node2D)Activator.CreateInstance(nodeType, baseConfig)!;
                subNode.SetParent(parent);
            }
        }

        public static void LoadTilemap(
            string texturePath,
            Rectangle region,
            int gridCellsX,
            int gridCellsY,
            int cellWidth,
            int cellHeight,
            IReadOnlyList<int> tileData,
            float layerDepth)
        {
            MTexture texture = new(texturePath);
            var textureRegion = texture.CreateSubTexture(region);

            var tileset = new Tileset(textureRegion, cellWidth, cellHeight);
            var tilemap = new Tilemap(tileset, gridCellsX, gridCellsY, 0.1f);

            for (int row = 0; row < gridCellsY; row++)
            {
                for (int col = 0; col < gridCellsX; col++)
                {
                    int index = row * gridCellsX + col;
                    int tile = (index >= 0 && index < tileData.Count) ? tileData[index] : 0;
                    tilemap.SetTile(col, row, tile);
                }
            }

            tilemap.LayerDepth = layerDepth;
        }

        public static void LoadTilemapFromJson(
            ContentManager content,
            string filename,
            string texturePath,
            string region)
        {
            var root = LoadJson(filename);

            var split = region.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse).ToArray();
            var rect = new Rectangle(split[0], split[1], split[2], split[3]);

            var layer = root.layers.FirstOrDefault(l => l.data != null && l.tileset != null)
                ?? throw new Exception("No tile layer found in JSON.");

            float layerDepth = float.Parse(layer.name, CultureInfo.InvariantCulture);

            LoadTilemap(
                texturePath: texturePath,
                region: rect,
                gridCellsX: layer.gridCellsX,
                gridCellsY: layer.gridCellsY,
                cellWidth: layer.gridCellWidth,
                cellHeight: layer.gridCellHeight,
                tileData: layer.data,
                layerDepth: layerDepth
            );
        }


        private static Node2D CreateNestedNode(Type nestedNodeType, OgmoFileInfo.Entity entity)
        {
            var ctor = nestedNodeType.GetConstructors().First();
            var configType = ctor.GetParameters().First().ParameterType;

            var nestedCfg = Activator.CreateInstance(configType)!;

            return (Node2D)Activator.CreateInstance(nestedNodeType, nestedCfg)!;
        }


        /// <summary>
        /// Instantiates all entities, decals, and tilemaps if parameters are provided
        /// </summary>
        public static void FromFile(
            string filename,
            string defaultTileTexture = null,
            string defaultTileRegion = null)
        {
            SearchForNodes(filename);
            SearchForDecals(filename);

            if (!string.IsNullOrEmpty(defaultTileTexture) && !string.IsNullOrEmpty(defaultTileRegion))
            {
                LoadTilemapFromJson(Engine.Content, filename, defaultTileTexture, defaultTileRegion);
            }
        }
    }
}
