using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class PoliceSorguProcedurModel
    {
        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public string PDFDosyasi { get; set; }
        public string PDFPolice { get; set; }
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string MusteriTemsilcisi { get; set; }
        public string Acente { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }
        public string Sigortali { get; set; }
        public string SigortaEttiren { get; set; }
        public string OzelAlan { get; set; }
        public decimal? BrutPrim { get; set; }
        public DateTime? PoliceBaslangicTarihi { get; set; }
        public DateTime? PoliceBitisTarihi { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public byte? OdemeTipi { get; set; }
        public byte? OdemeSekli { get; set; }
        public byte? Taksit { get; set; }
    }


    public class PoliceHedefRaporModel
    {
        public string Acente { get; set; }
        public string TaliAcente { get; set; }
        public string Brans { get; set; }

        public decimal GerceklesenPoliceAdedi1 { get; set; }
        public decimal GerceklesenPrim1 { get; set; }
        public decimal HedefPoliceAdedi1 { get; set; }
        public decimal HedefPrim1 { get; set; }
        public decimal AdetYuzde1 { get; set; }
        public decimal PrimYuzde1 { get; set; }

        public decimal GerceklesenPoliceAdedi2 { get; set; }
        public decimal GerceklesenPrim2 { get; set; }
        public decimal HedefPoliceAdedi2 { get; set; }
        public decimal HedefPrim2 { get; set; }
        public decimal AdetYuzde2 { get; set; }
        public decimal PrimYuzde2 { get; set; }

        public decimal GerceklesenPoliceAdedi3 { get; set; }
        public decimal GerceklesenPrim3 { get; set; }
        public decimal HedefPoliceAdedi3 { get; set; }
        public decimal HedefPrim3 { get; set; }
        public decimal AdetYuzde3 { get; set; }
        public decimal PrimYuzde3 { get; set; }

        public decimal GerceklesenPoliceAdedi4 { get; set; }
        public decimal GerceklesenPrim4 { get; set; }
        public decimal HedefPoliceAdedi4 { get; set; }
        public decimal HedefPrim4 { get; set; }
        public decimal AdetYuzde4 { get; set; }
        public decimal PrimYuzde4 { get; set; }

        public decimal GerceklesenPoliceAdedi5 { get; set; }
        public decimal GerceklesenPrim5 { get; set; }
        public decimal HedefPoliceAdedi5 { get; set; }
        public decimal HedefPrim5 { get; set; }
        public decimal AdetYuzde5 { get; set; }
        public decimal PrimYuzde5 { get; set; }

        public decimal GerceklesenPoliceAdedi6 { get; set; }
        public decimal GerceklesenPrim6 { get; set; }
        public decimal HedefPoliceAdedi6 { get; set; }
        public decimal HedefPrim6 { get; set; }
        public decimal AdetYuzde6 { get; set; }
        public decimal PrimYuzde6 { get; set; }

        public decimal GerceklesenPoliceAdedi7{ get; set; }
        public decimal GerceklesenPrim7 { get; set; }
        public decimal HedefPoliceAdedi7 { get; set; }
        public decimal HedefPrim7 { get; set; }
        public decimal AdetYuzde7 { get; set; }
        public decimal PrimYuzde7 { get; set; }

        public decimal GerceklesenPoliceAdedi8 { get; set; }
        public decimal GerceklesenPrim8 { get; set; }
        public decimal HedefPoliceAdedi8 { get; set; }
        public decimal HedefPrim8 { get; set; }
        public decimal AdetYuzde8 { get; set; }
        public decimal PrimYuzde8 { get; set; }

        public decimal GerceklesenPoliceAdedi9 { get; set; }
        public decimal GerceklesenPrim9 { get; set; }
        public decimal HedefPoliceAdedi9 { get; set; }
        public decimal HedefPrim9 { get; set; }
        public decimal AdetYuzde9 { get; set; }
        public decimal PrimYuzde9 { get; set; }

        public decimal GerceklesenPoliceAdedi10 { get; set; }
        public decimal GerceklesenPrim10 { get; set; }
        public decimal HedefPoliceAdedi10 { get; set; }
        public decimal HedefPrim10 { get; set; }
        public decimal AdetYuzde10 { get; set; }
        public decimal PrimYuzde10 { get; set; }

        public decimal GerceklesenPoliceAdedi11 { get; set; }
        public decimal GerceklesenPrim11 { get; set; }
        public decimal HedefPoliceAdedi11 { get; set; }
        public decimal HedefPrim11 { get; set; }
        public decimal AdetYuzde11 { get; set; }
        public decimal PrimYuzde11 { get; set; }

        public decimal GerceklesenPoliceAdedi12 { get; set; }
        public decimal GerceklesenPrim12 { get; set; }
        public decimal HedefPoliceAdedi12 { get; set; }
        public decimal HedefPrim12 { get; set; }
        public decimal AdetYuzde12 { get; set; }
        public decimal PrimYuzde12 { get; set; }

    }

    public class PoliceHedefRaporEkranModel
    {
        public string Acente { get; set; }
        public string TaliAcente { get; set; }
        public string Brans { get; set; }

        public int    GerceklesenPoliceAdediOcak { get; set; }
        public decimal GerceklesenPrimOcak { get; set; }
        public int    HedefPoliceAdediOcak { get; set; }
        public decimal HedefPrimOcak { get; set; }
        public decimal AdetYuzdeOcak { get; set; }
        public decimal PrimYuzdeOcak { get; set; }

        public int GerceklesenPoliceAdediSubat { get; set; }
        public decimal GerceklesenPrimSubat { get; set; }
        public int HedefPoliceAdediSubat { get; set; }
        public decimal HedefPrimSubat { get; set; }
        public decimal AdetYuzdeSubat { get; set; }
        public decimal PrimYuzdeSubat { get; set; }

        public int GerceklesenPoliceAdediMart { get; set; }
        public decimal GerceklesenPrimMart { get; set; }
        public int HedefPoliceAdediMart { get; set; }
        public decimal HedefPrimMart { get; set; }
        public decimal AdetYuzdeMart { get; set; }
        public decimal PrimYuzdeMart { get; set; }

        public int GerceklesenPoliceAdediNisan { get; set; }
        public decimal GerceklesenPrimNisan { get; set; }
        public int HedefPoliceAdediNisan { get; set; }
        public decimal HedefPrimNisan { get; set; }
        public decimal AdetYuzdeNisan { get; set; }
        public decimal PrimYuzdeNisan { get; set; }

        public int GerceklesenPoliceAdediMayis { get; set; }
        public decimal GerceklesenPrimMayis { get; set; }
        public int HedefPoliceAdediMayis { get; set; }
        public decimal HedefPrimMayis { get; set; }
        public decimal AdetYuzdeMayis { get; set; }
        public decimal PrimYuzdeMayis { get; set; }

        public int GerceklesenPoliceAdediHaziran { get; set; }
        public decimal GerceklesenPrimHaziran { get; set; }
        public int HedefPoliceAdediHaziran { get; set; }
        public decimal HedefPrimHaziran { get; set; }
        public decimal AdetYuzdeHaziran { get; set; }
        public decimal PrimYuzdeHaziran { get; set; }

        public int GerceklesenPoliceAdediTemmuz { get; set; }
        public decimal GerceklesenPrimTemmuz { get; set; }
        public int HedefPoliceAdediTemmuz { get; set; }
        public decimal HedefPrimTemmuz { get; set; }
        public decimal AdetYuzdeTemmuz { get; set; }
        public decimal PrimYuzdeTemmuz { get; set; }

        public int GerceklesenPoliceAdediAgustos { get; set; }
        public decimal GerceklesenPrimAgustos { get; set; }
        public int HedefPoliceAdediAgustos { get; set; }
        public decimal HedefPrimAgustos { get; set; }
        public decimal AdetYuzdeAgustos { get; set; }
        public decimal PrimYuzdeAgustos { get; set; }

        public int GerceklesenPoliceAdediEylul { get; set; }
        public decimal GerceklesenPrimEylul { get; set; }
        public int HedefPoliceAdediEylul { get; set; }
        public decimal HedefPrimEylul { get; set; }
        public decimal AdetYuzdeEylul { get; set; }
        public decimal PrimYuzdeEylul { get; set; }

        public int GerceklesenPoliceAdediEkim { get; set; }
        public decimal GerceklesenPrimEkim { get; set; }
        public int HedefPoliceAdediEkim { get; set; }
        public decimal HedefPrimEkim { get; set; }
        public decimal AdetYuzdeEkim { get; set; }
        public decimal PrimYuzdeEkim { get; set; }

        public int GerceklesenPoliceAdediKasim { get; set; }
        public decimal GerceklesenPrimKasim { get; set; }
        public int HedefPoliceAdediKasim { get; set; }
        public decimal HedefPrimKasim { get; set; }
        public decimal AdetYuzdeKasim { get; set; }
        public decimal PrimYuzdeKasim { get; set; }

        public int GerceklesenPoliceAdediAralik { get; set; }
        public decimal GerceklesenPrimAralik { get; set; }
        public int HedefPoliceAdediAralik { get; set; }
        public decimal HedefPrimAralik { get; set; }
        public decimal AdetYuzdeAralik { get; set; }
        public decimal PrimYuzdeAralik { get; set; }
    }
}
