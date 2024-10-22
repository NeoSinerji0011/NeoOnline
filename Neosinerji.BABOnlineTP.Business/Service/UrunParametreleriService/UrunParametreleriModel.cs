using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UrunParametreleriModel
    {
        #region AEGON

        #region TURUNCU ELMA

        public const string AEGON_TE_AnaTeminatLimiti_Dolar = "AEGON_TE_AnaTeminatLimiti_Dolar";
        public const string AEGON_TE_AnaTeminatLimiti_Euro = "AEGON_TE_AnaTeminatLimiti_Euro";
        public const string AEGON_TE_YillikPrimLimiti_Dolar = "AEGON_TE_YillikPrimLimiti_Dolar";
        public const string AEGON_TE_YillikPrimLimiti_Euro = "AEGON_TE_YillikPrimLimiti_Euro";

        public const string AEGON_HTED_USD_KUR_PARAM = "AEGON_HTED_USD_KUR_PARAM";
        public const string AEGON_HTED_EUR_KUR_PARAM = "AEGON_HTED_EUR_KUR_PARAM";
        public const string AEGON_KHYT_USD_KUR_PARAM = "AEGON_KHYT_USD_KUR_PARAM";
        public const string AEGON_KHYT_EUR_KUR_PARAM = "AEGON_KHYT_EUR_KUR_PARAM";
        public const string AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR = "AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR";
        public const string AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD = "AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD";


        public static string[] Bundle_AEGON_TE = new string[] { 
            //UrunParametreleriModel.AEGON_HTED_USD_KUR_PARAM,
            //UrunParametreleriModel.AEGON_HTED_EUR_KUR_PARAM,
            //UrunParametreleriModel.AEGON_KHYT_USD_KUR_PARAM,
            //UrunParametreleriModel.AEGON_KHYT_EUR_KUR_PARAM,
            //UrunParametreleriModel.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR,
            //UrunParametreleriModel.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD,
            UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Dolar,
            UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Euro,
            UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Dolar,
            UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Euro,
        };

        #endregion

        #endregion
    }

    public class UrunParamsTable
    {
        UrunParametreleri[] _Params;

        public UrunParamsTable(UrunParametreleri[] param)
        {
            _Params = param;
        }

        public string this[string index]
        {
            get
            {
                if (_Params == null)
                    return String.Empty;

                UrunParametreleri parameter = _Params.FirstOrDefault(w => w.Kod == index);
                if (parameter != null)
                    return parameter.Deger;

                return String.Empty;
            }
        }

        public int Count
        {
            get
            {
                return this._Params.Length;
            }
        }
    }
}
