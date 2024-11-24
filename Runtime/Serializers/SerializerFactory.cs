namespace ActionCode.Persistence
{
    public static class SerializerFactory
    {
        public static ISerializer Create(SerializerType type)
        {
            return type switch
            {
                SerializerType.Binary => new BinarySerializer(),
                SerializerType.JsonUtility => new JsonUtilitySerializer(),
                SerializerType.JsonNewtonsoft => new NewtonsoftSerializer(),
                SerializerType.UnityJsonSerialization => new UnityJsonSerialization(),
                SerializerType.Xml => new XmlSerializer(),
                _ => null,
            };
        }
    }
}