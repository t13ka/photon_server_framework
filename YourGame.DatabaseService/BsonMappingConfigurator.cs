namespace YourGame.DatabaseService
{
    using MongoDB.Bson.Serialization;

    using YourGame.Common.Domain;

    /// <summary>
    /// </summary>
    public static class BsonMappingConfigurator
    {
        /// <summary>
        /// </summary>
        public static void Configure()
        {
            var domainConfig = new DomainConfiguration();

            BsonClassMap.RegisterClassMap<IEntity>(
                cm =>
                    {
                        cm.AutoMap();
                        cm.SetIsRootClass(true);
                        cm.MapIdField("_id");
                    });
        }
    }
}