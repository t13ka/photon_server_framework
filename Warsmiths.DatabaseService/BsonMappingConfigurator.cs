namespace YourGame.DatabaseService
{
    using MongoDB.Bson.Serialization;

    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Craft.Quest;

    /// <summary>
    /// </summary>
    public static class BsonMappingConfigurator
    {
        /// <summary>
        /// </summary>
        public static void Configure()
        {
            var domainConfig = new DomainConfiguration(true);

            BsonClassMap.RegisterClassMap<IEntity>(
                cm =>
                    {
                        cm.AutoMap();
                        cm.SetIsRootClass(true);
                        cm.MapIdField("_id");
                    });

            // mapping for all objects
            foreach (var baseEquipment in domainConfig.Objects)
            {
                BsonClassMap.LookupClassMap(baseEquipment.GetType());
            }

            BsonClassMap.LookupClassMap(typeof(BaseReciept));
            BsonClassMap.LookupClassMap(typeof(BaseQuest));

            foreach (var b in domainConfig.Items)
            {
                BsonClassMap.LookupClassMap(b.GetType());
            }
        }
    }
}