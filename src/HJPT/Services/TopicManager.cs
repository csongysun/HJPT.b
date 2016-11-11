

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Csys.Common;
using HJPT.Data;
using HJPT.Models;
using Microsoft.AspNetCore.Http;
using System;

using FilterDictionary = System.Collections.Generic.Dictionary<string, System.Func<System.Linq.IQueryable<HJPT.Models.Topic>, string, System.Linq.IQueryable<HJPT.Models.Topic>>>;

using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace HJPT.Services
{
    public class TopicManager
    {

        private TorrentManager _torrent;
        private ITopicStore _store;
        public TopicManager(
            TorrentManager torrent,
            ITopicStore store
        )
        {
            _torrent = torrent;
            _store = store;
            FilterInit();
        }

        public List<Topic> GetTopics(IQueryCollection query)
        {
            var topics = _store.Topics;

            //filter
            foreach (var kv in query)
            {
                if (filters.ContainsKey(kv.Key))
                {
                    topics = filters[kv.Key](topics, kv.Value);
                }
            }

            StringValues val;

            //order
            int order;
            bool isd;
            if (query.TryGetValue("order", out val))
            {
                order = int.Parse(val);
            }
            else
            {
                order = 1;
            }

            if (order > 5 || order < -5 || order == 0) order = 1;
            if (order > 0)
                isd = false;
            else
            {
                order = -order;
                isd = false;
            }

            if (isd)
            {
                topics = topics.OrderBy(orders[order]);
            }
            else
                topics = topics.OrderByDescending(orders[order]);


            //paging
            int page, count;

            if (query.TryGetValue("page", out val))
            {
                page = int.Parse(val);
            }
            else
            {
                page = 1; //default
            }

            if (query.TryGetValue("count", out val))
            {
                count = int.Parse(val);
                //set max value or not

            }
            else
            {
                count = 50;
            }

            topics = topics.Skip((page - 1) * count).Take(count);

            return topics.ToList();
        }

        // private Dictionary<string, Func<string, string>> filters;
        // private void FilterInit(){
        //     filters = new Dictionary<string, Func<string, string>>();
        //     filters.Add("category", val => $" AND \"CategoryId\" = {int.Parse(val)}");
        //     //filters.Add("category", val => $" AND \"CategoryId\" = {val}");
        //     //filters.Add("category", val => $" AND \"CategoryId\" = {val}");
        //     //filters.Add("category", val => $" AND \"CategoryId\" = {val}");

        // }

        private static FilterDictionary filters;
        private static List<Expression<Func<Topic, object>>> orders;

        private void FilterInit()
        {
            filters = new FilterDictionary();
            filters.Add("category", (topics, val) => topics.Where(t => t.CategoryId == int.Parse(val)));
            //filters.Add("category", (topics, val) => topics.Where(t => t.CategoryId == int.Parse(val)));
            //filters.Add("category", (topics, val) => topics.Where(t => t.CategoryId == int.Parse(val)));
            //filters.Add("category", (topics, val) => topics.Where(t => t.CategoryId == int.Parse(val)));

            orders = new List<Expression<Func<Topic, object>>>();
            orders.Add(t => t.AddDate);
            orders.Add(t => t.Torrent.Seeder);
            orders.Add(t => t.Torrent.Leecher);
        }

    }
}

