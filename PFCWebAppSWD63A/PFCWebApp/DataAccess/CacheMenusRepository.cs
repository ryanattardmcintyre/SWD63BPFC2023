using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PFCWebApp.Models;
using StackExchange.Redis;

namespace PFCWebApp.DataAccess
{
    public class CacheMenusRepository
    {
        IDatabase myDb;
        public CacheMenusRepository(string connectionString)
        {
            var cm = ConnectionMultiplexer.Connect(connectionString);
            myDb = cm.GetDatabase();
        }


        public async Task<List<Menu>> GetMenus()
        {
            string menusStr = await myDb.StringGetAsync("menus");
            if (string.IsNullOrEmpty(menusStr) == false)
            {
                var output = JsonConvert.DeserializeObject<List<Menu>>(menusStr);
                return output;
            }
            else return new List<Menu>();
        }


        public async Task<bool> AddMenu(Menu m)
        {
            var list = await GetMenus();
            list.Add(m);

            var menusStr = JsonConvert.SerializeObject(list);

            return await myDb.StringSetAsync("menus", menusStr);
        }

    }
}
