﻿using System;
using Artemis.Core;
using Artemis.Core.ScriptingProviders;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Ninject;
using Ninject.Parameters;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Scripts
{
    public class JavaScriptProfileScript : ProfileScript
    {
        private readonly PluginJintEngine _engine;

        public JavaScriptProfileScript(Profile profile, Plugin plugin, ScriptConfiguration configuration) : base(profile, configuration)
        {
            ScriptConfiguration.ScriptContentChanged += ConfigurationOnScriptContentChanged;

            _engine = plugin.Kernel!.Get<PluginJintEngine>(new ConstructorArgument("script", this));
            _engine.ExecuteScript();
        }

        private void ConfigurationOnScriptContentChanged(object sender, EventArgs e)
        {
            _engine.ExecuteScript();
        }

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ScriptConfiguration.ScriptContentChanged -= ConfigurationOnScriptContentChanged;
                _engine.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}