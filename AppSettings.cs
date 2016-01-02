using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf
{
    /// <summary>
    /// アプリケーション設定データ管理
    /// </summary>
    public class AppSettings : AppSettingsBase
    {
        private static readonly AppSettings _current = new AppSettings();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AppSettings()
        {
        }

        public static AppSettings Current
        {
            get { return _current; }
        }
        
        public string[] FolderTokenList
        {
            get
            {
                string[] result = GetValue<string[]>(ContainerType.Roaming);
                if (result == null)
                {
                    return new string[0];
                }
                return result;
            }
            set
            {
                SetValue(value, ContainerType.Roaming);
                OnPropertyChanged();
            }
        }
    }
}
