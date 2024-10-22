using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IUlkeService
    {
        Ulke GetUlke(string ulkeKodu);
        string GetUlkeAdi(string ulkeKodu);
        List<Ulke> GetUlkeList();
        Ulke CreateUlke(Ulke ulke);
        void UpdateUlke(Ulke ulke);

        Il GetIl(string ulkeKodu, string ilKodu);
        string GetIlAdi(string ulkeKodu, string ilKodu);
        List<Il> GetIlList();
        List<Il> GetIlList(string ulkeKodu);
        Il CreateIl(Il il);
        void UpdateIl(Il il);

        Ilce GetIlce(int ilceKodu);
        string GetIlceAdi(int ilceKodu);
        List<Ilce> GetIlceList();
        List<Ilce> GetIlceList(string ulkeKodu, string ilKodu);
        Ilce CreateIlce(Ilce ilce);
        void UpdateIlce(Ilce ilce);
        ErgoIlIlce GetErgoIlIlce(string ilAdi, string ilceAdi);

    }
}
