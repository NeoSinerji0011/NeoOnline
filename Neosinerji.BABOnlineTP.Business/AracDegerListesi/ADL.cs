using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;

namespace Neosinerji.BABOnlineTP.Business
{
    public class ADL
    {
        private string mFilePath = String.Empty;
        private string mData = String.Empty;
        private IAracContext _AracContext;
        private List<AracDegerleriExcelModel> model;

        public void Init(string source)
        {
            mFilePath = source;
        }

        public void Start()
        {
            ProcessFile();
        }

        private bool ProcessFile()
        {
            XL2007 xl = null;
            try
            {
                xl = new XL2007(mFilePath, 1);

                if (xl.Open())
                {
                    _AracContext = DependencyResolver.Current.GetService<IAracContext>();
                    mContents = xl.GetRowContents(ROW_CAPTIONS, -1);
                    mColumnCount = mContents.Count;

                    if (mColumnCount < COLUMN_FIRSTPRICE)
                        throw new Exception("Dosya formatı tanınamadı!");

                    this.TotalRowCount = xl.GetRowCount(ROW_CAPTIONS + 1);
                    model = new List<AracDegerleriExcelModel>();
                    PrepareModels();

                    this.ClearTempTable();

                    this.CurrentRow = 0;
                    int row = ROW_CAPTIONS + 1;
                    do
                    {
                        mRowContents = xl.GetRowContents(row, mColumnCount);
                        if (mRowContents == null) break;

                        try
                        { 
                            ProcessRow();
                        }
                        catch (Exception ex)
                        {
                        }
                        row++;
                        this.CurrentRow++;
                    }
                    while (mRowContents.Count > COLUMN_FIRSTPRICE);

                    this.CompleteTransfer();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
            finally
            {
                if (xl != null)
                {
                    xl.Dispose();
                    xl = null;
                }
            }

            return false;
        }

        private void ProcessRow()
        {
            AracDegerleriExcelModel mdl = new AracDegerleriExcelModel();

            mdl.MarkaKodu = mRowContents[COLUMN_BRANDCODE];
            mdl.tipKodu = mRowContents[COLUMN_TYPECODE];
            mdl.markaAdi = mRowContents[COLUMN_BRAND];
            mdl.tipAdi = mRowContents[COLUMN_TYPE];
            int indis = 4;
            mdl.yil2023 = mRowContents[indis]; indis++;
            mdl.yil2022 = mRowContents[indis]; indis++;
            mdl.yil2021 = mRowContents[indis]; indis++;
            mdl.yil2020 = mRowContents[indis]; indis++;
            mdl.yil2019 = mRowContents[indis]; indis++;
            mdl.yil2018 = mRowContents[indis]; indis++;
            mdl.yil2017 = mRowContents[indis]; indis++;
            mdl.yil2016 = mRowContents[indis]; indis++;
            mdl.yil2015 = mRowContents[indis]; indis++;
            mdl.yil2014 = mRowContents[indis]; indis++;
            mdl.yil2013 = mRowContents[indis]; indis++;
            mdl.yil2012 = mRowContents[indis]; indis++;
            mdl.yil2011 = mRowContents[indis]; indis++;
            mdl.yil2010 = mRowContents[indis]; indis++;
            mdl.yil2009 = mRowContents[indis]; 
              
            model.Add(mdl);

            if (model.Count == TotalRowCount)
            {
                bool sonuc = _AracContext.AracDegerleriGonder(model);
            }
        }

        private void PrepareModels()
        {
            List<string> models = new List<string>();

            for (int col = COLUMN_FIRSTPRICE; col < mContents.Count; col++)
            {
                string s = mContents[col];

                string result = String.Empty;
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]))
                        result += s[i];
                }

                models.Add(result);
            }

            mModels = models;
        }

        private void ClearTempTable()
        {
        }

        private void CompleteTransfer()
        {
        }

        #region private properties
        private const int ROW_CAPTIONS = 1;
        private const int COLUMN_BRANDCODE = 0;
        private const int COLUMN_TYPECODE = 1;
        private const int COLUMN_BRAND = 2;
        private const int COLUMN_TYPE = 3;
        private const int COLUMN_FIRSTPRICE = 4;


        private List<string> mContents = null;
        private List<string> mModels = null;
        private List<string> mRowContents = null;
        private int mColumnCount = 0;

        private string mConnectionString = String.Empty;
        private int mCurrentRow;
        private int mTotalRowCount;

        private int CurrentRow
        {
            get { return mCurrentRow; }
            set
            {
                mCurrentRow = value;

                if (mCurrentRow % 100 == 0)
                {
                    int percentage = (int)(((double)mCurrentRow / (double)this.TotalRowCount) * 80);
                }
            }
        }

        private int TotalRowCount
        {
            get { return mTotalRowCount; }
            set { mTotalRowCount = value; }
        }
        #endregion
    }
}
