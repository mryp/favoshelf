using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace favoshelf
{
    /// <summary>
    /// ナビゲーションメニューアイテム
    /// </summary>
    public class NavMenuItem
    {
        /// <summary>
        /// ラベル
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// アイコンシンボル
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// シンボル名
        /// </summary>
        public char SymbolAsChar
        {
            get
            {
                return (char)this.Symbol;
            }
        }

        /// <summary>
        /// 表示するページ型
        /// </summary>
        public Type DestPage { get; set; }

        /// <summary>
        /// 引数
        /// </summary>
        public object Arguments { get; set; }
    }
}
