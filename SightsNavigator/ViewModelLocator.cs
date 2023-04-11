using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator
{
    public static class ViewModelLocator
    {
        private static Dictionary<string, object> _viewModels = new Dictionary<string, object>();

        public static T GetViewModel<T>(string key) where T : class
        {
            if (_viewModels.ContainsKey(key))
            {
                return _viewModels[key] as T;
            }

            return null;
        }

        public static void SetViewModel<T>(string key, T viewModel) where T : class
        {
            if (_viewModels.ContainsKey(key))
            {
                _viewModels[key] = viewModel;
            }
            else
            {
                _viewModels.Add(key, viewModel);
            }
        }
    }

}
