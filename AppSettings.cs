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

        /// <summary>
        /// 共通で使用するカレントオブジェクト
        /// </summary>
        public static AppSettings Current
        {
            get { return _current; }
        }
        
    }
}
