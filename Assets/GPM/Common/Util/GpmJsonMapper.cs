using Gpm.Common.ThirdParty.LitJson;
using System;
using System.IO;

namespace Gpm.Common.Util
{
    public static class GpmJsonMapper
    {
        public static string ToJson(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public static void ToJson(object obj, JsonWriter writer)
        {
            JsonMapper.ToJson(obj, writer);
        }

        public static JsonData ToObject(JsonReader reader)
        {
            return JsonMapper.ToObject(reader);
        }

        public static JsonData ToObject(TextReader reader)
        {
            return JsonMapper.ToObject(reader);
        }

        public static JsonData ToObject(string json)
        {
            return JsonMapper.ToObject(json);
        }

        public static T ToObject<T>(JsonReader reader)
        {
            return JsonMapper.ToObject<T>(reader);
        }

        public static T ToObject<T>(TextReader reader)
        {
            return JsonMapper.ToObject<T>(reader);
        }

        public static T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }

        public static object ToObject(string json, Type convertType)
        {
            return JsonMapper.ToObject(json, convertType);
        }

        public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
        {
            return JsonMapper.ToWrapper(factory, reader);
        }

        public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
        {
            return JsonMapper.ToWrapper(factory, json);
        }

        public static void RegisterExporter<T>(ExporterFunc<T> exporter)
        {
            JsonMapper.RegisterExporter<T>(exporter);
        }

        public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
        {
            JsonMapper.RegisterImporter(importer);
        }

        public static void UnregisterExporters()
        {
            JsonMapper.UnregisterExporters();
        }

        public static void UnregisterImporters()
        {
            JsonMapper.UnregisterImporters();
        }
    }
}