namespace Gpm.Common.Util
{
    using System.Collections.Generic;
    using ThirdParty.MessagePack;
    using ThirdParty.MessagePack.Resolvers;
    using ThirdParty.MessagePack.Unity;

    public static class GpmMessagePackMapper
    {
        public static byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public static byte[] SerializeUnsafe<T>(T obj)
        {
            return MessagePackSerializer.SerializeUnsafe<T>(obj).Array;
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public static void Initialize(params IFormatterResolver[] customResolvers)
        {
            List<IFormatterResolver> resolverList = new List<IFormatterResolver>();
            foreach (var resolver in customResolvers)
            {
                resolverList.Add(resolver);
            }
            resolverList.Add(BuiltinResolver.Instance);
            resolverList.Add(AttributeFormatterResolver.Instance);
            resolverList.Add(PrimitiveObjectResolver.Instance);
            resolverList.Add(UnityResolver.Instance);

            CompositeResolver.RegisterAndSetAsDefault(resolverList.ToArray());
        }
    }
}