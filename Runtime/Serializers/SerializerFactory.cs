namespace ActionCode.Persistence
{
    public static class SerializerFactory
    {
        public static ISerializer Create(SerializerType type)
        {
            return type switch
            {
                SerializerType.Binary => BinarySerializer.IsAvailable() ? new BinarySerializer() : null,
                SerializerType.JsonUtility => JsonUtilitySerializer.IsAvailable() ? new JsonUtilitySerializer() : null,
                SerializerType.JsonNewtonsoft => throw new System.NotImplementedException(),
                SerializerType.Xml => new XmlSerializer(),
                _ => null,
            };
        }
    }
}