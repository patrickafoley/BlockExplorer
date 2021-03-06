﻿using System;
using System.Collections.Generic;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.BlockStore;

namespace Stratis.Bitcoin.Features.IndexStore
{
    /// <summary>
    /// Configuration related to storage of transactions.
    /// </summary>
    public class IndexSettings : StoreSettings
    {
        private Action<IndexSettings> callback = null;
        public Dictionary<string, IndexExpression> Indexes { get; private set; }

        public IndexSettings()
            : base()
        {
            this.Indexes = new Dictionary<string, IndexExpression>();
        }

        public IndexSettings(Action<IndexSettings> callback)
            : this()
        {
            this.callback = callback;
        }

        public void RegisterIndex(string name, string builder, bool multiValue, string[] dependencies = null)
        {
            this.Indexes[name] = new IndexExpression(multiValue, builder, dependencies);
        }

        /// <summary>
        /// Loads the storage related settings from the application configuration.
        /// </summary>
        /// <param name="config">Application configuration.</param>
        public override void Load(NodeSettings nodeSettings)
        {
            var config = nodeSettings.ConfigReader;

            this.Prune = false;
            this.TxIndex = true;

            this.callback?.Invoke(this);
        }
    }
}