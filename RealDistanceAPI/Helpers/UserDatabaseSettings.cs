﻿using System;
namespace ApiUsers.Helpers
{
    public class UsersDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
    }
}

