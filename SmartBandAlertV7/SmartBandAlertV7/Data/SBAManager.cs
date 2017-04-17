using Microsoft.WindowsAzure.MobileServices;
using System;


namespace SmartBandAlertV7.Data
{
    public partial class SBAManager
    {
        static SBAManager defaultInstance = new SBAManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<TodoItem> todoTable;


#else
        IMobileServiceTable<String> todoTable;
#endif

        private SBAManager()
        {
            this.client = new MobileServiceClient("https://sbat1.azurewebsites.net");

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<TodoItem>();
#else
            this.todoTable = client.GetTable<String>();
#endif
        }

        public static SBAManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

    }
}
