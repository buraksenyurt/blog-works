---
layout: post
title: "Instagram REST Servislerinin .Net Tarafında Basit Kullanımı"
date: 2016-05-04 06:00:00 +0300
categories:
  - rest
tags:
  - instagram
  - rest-api
  - WebClient
  - json
  - csharp
  - jquery
  - javascript
  - endpoint-behavior
  - api
  - nuget
---
Neredeyse hepimizin sosyal ağ üzerinde hesapları bulunuyor. Facebook, Twitter, Instagram, Youtube, Flickr ve benzerlerini bunlara örnek olarak verebiliriz. Bu ağlar pek tabii kendi hizmetlerini geliştiricilerin kullanımına da uzun zamandır açmış durumdalar. Geliştirici olarak bizleri bu kısım daha çok ilgilendiriyor.

![UsingInstagram_3.gif](/assets/images/2016/UsingInstagram_3.gif)

Peki bu tip ağlar geliştiricilere kendi hizmetlerini nasıl sunuyorlar? Bunun en bilinen ve ortak yanı pek tabii ki servisler şeklinde yayınlanmaları. Burada kullanılacak olan servislerin istemci uygulamaların ne tipte olduğuna bakılmaksızın ortak bir standartta çalışması önemli. İşte bu noktada devreye REST (Representational State Transfer) yaklaşımını benimseyen servisler geliyor. Neredeyse pek çok sosyal ağın REST tipinden hizmet veren Endpoint'leri mevcut. Bu sayede istemci teknolojilerinden tamamen bağımsız olarak dış dünyaya fonksiyonellikler sunulabilmekte. İşte bu yazımızda Instagram'ın dışarıya sunduğu bu REST servisleri nasıl kullanabileceğimizi basit bir Endpoint üzerinden incelemeye çalışacağız. Amacımız Instagram hesabı ile giriş yapan bir kullanıcının eklediği son fotoğrafları web sayfası üzerinde gösterbilmek.

> Aslında Instagram API'sinin.Net dünyasında daha kolay kullanımını sağlayan NuGet paketleri de mevcut. Kod tarafında nesne odaklı modeli kullanmak çok daha mantıklı. Ancak bizim amacımız REST taleplerini Instagram servislerine yaparken çalışma mimarisini de anlamaya çalışmak. Yazıyı bu şekilde değerlendirmenizi tavsiye ederim.

Dilerseniz hiç vakit kaybetmeden işlemlerimize başlayalım.

## Instagram'a Uygulamamızın Kayıt Edilmesi (Registration)

Öncelikle Instagram'a gidip uygulamamız için bir kayıt işlemi (Register) gerçekleştirmeli ve kullanıcı girişi sonrası yönlendirilecek olan sayfa adresini bildirmeliyiz (Redirect URL).

![UsingInstagram_1.gif](/assets/images/2016/UsingInstagram_1.gif)

Bu kayıt işleminde önemli olan bir kaç nokta var. Client ID, Client Secret ve Redirect URI değerleri istemci uygulama tarafından kullanılacak değişkenler olacak. Geliştireceğimiz örnekte bu değerleri ele alarak Instagram'a erişim hakkı olan bir jeton (Access Token) elde edeceğiz. Bir başka deyişle uygulamanın REST Endoint'lerini kullanabilmesi için gerekli izni alacağız. Özellikle Redirect URI değeri önemli. Nitekim uygulamamız üzerinden bir Login işlemi de gerçekleştirilmekte. Bu Login işlemi sonrasında geriye dönülecek olan URL bilgisi, Instagram'a kayıt edilen adres ile eş olmalı. Aksi durumda bir Authentication hatası alırız.

> Instagram API'sinin kullanımı ile ilişkili oldukça detaylı bir doküman da mevcut. Plaftorm bağımsız olarak yazılmış dokümanı inceleyerek kullanılabilecek [REST Endpoint'leri hakkında detaylı bilgiye](https://developers.facebook.com/docs/instagram) ulaşabilirsiniz.

## Uygulama Kodlarının Geliştirilmesi

Şimdi gelin boş bir web uygulaması açıp içerisine koyacağımız Default.aspx sayfasının kodlarını geliştirmeye başlayalım ve Instagram'a giriş yapan kişinin (ki bu senaryoda ben oluyorum) paylaştığı son fotoğrafları ekrana basalım.

Default.aspx.cs içeriğimiz şu şekildedir.

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web.UI;

namespace InstagramRESTHello
{
    public partial class Default : System.Web.UI.Page
    {
        static string code = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["code"]) && !Page.IsPostBack)
            {
                code = Request["code"].ToString();
                GetUserIdAndAccessToken();
            }
        }

        public void GetUserIdAndAccessToken()
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("client_id", ConfigurationManager.AppSettings["ClientID"].ToString());
            parameters.Add("client_secret", ConfigurationManager.AppSettings["ClientSecret"].ToString());
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("redirect_uri", ConfigurationManager.AppSettings["RedirectUri"].ToString());
            parameters.Add("code", code);

            WebClient client = new WebClient();
            var result = client.UploadValues("https://api.instagram.com/oauth/access_token", "POST", parameters);
            var response = System.Text.Encoding.Default.GetString(result);

            var returnContent = (JObject)JsonConvert.DeserializeObject(response);
            string accessToken = (string)returnContent["access_token"];
            string id = returnContent["user"]["id"].ToString();

            var script = string.Format("<script>var userId = \"{0}\"; var accessToken = \"{1}\";</script>",
                id,
                accessToken
                );
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GetToken",script);
        }

        protected void btnGetLastPhotos_Click(object sender, EventArgs e)
        {
            var clientId = ConfigurationManager.AppSettings["ClientID"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            var redirectPage = ConfigurationManager.AppSettings["RedirectUri"];

            var loginUrl = string.Format("https://api.instagram.com/oauth/authorize/?client_id={0}&redirect_uri={1}&response_type=code"
                , clientId, redirectPage);

            Response.Redirect(loginUrl);
        }
    }
}
```

> Instagram REST servisleri varsayılan olarak JSON formatından içerik döndürmektedir. Bu nedenle gelen cevapların ayrıştırılmasını kolaylaştırmak amacıyla Newtonsoft.JSON paketinden yararlanılmaktadır. Bu paketi yüklemeyi unutmayalım. NuGet Console'dan yüklemek için install-package Newtonsoft.Json yazmamız yeterlidir.

Kodumuzda neler yaptığımızı kısaca anlatalım. Öncelikle uygulamamızın Instagram API'sinden yararlanabilmesini sağlamak için izin almamız gerekir. Bu izin için [https://api.instagram.com/oauth/access_token](https://api.instagram.com/oauth/access_token) adresine HTTP POST tipinden bir paket gönderilir. Instagram'a kayıt ettiğimiz uygulama ile ilgili bilgiler gönderilecek pakete anahtar-değer (key-value) koleksiyonu şeklinde eklenir. clientid, clientsecret, granttype, redirecturi ve code. Eğer POST işlemi sonucu başarılı ise gelen cevap JSON formatında ters serileştirilerek accesstoken ve id bilgileri yakalanır. Bu bilgiler sayfaya register edilecek bir Javascript fonksiyonu için gereklidir. Nitekim çekilen id ve accesstoken bilgisini jQuery içerisindeki REST çağrısında kullanacağız.

Gelelim Default.aspx içeriğine.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="InstagramRESTHello.Default" %>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/2.2.2/jquery.min.js" type="text/javascript"></script>

<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnGetLastPhotos" Text="Son Fotoğrafları Getir" runat="server" OnClick="btnGetLastPhotos_Click" />
        </div>
        <div>
            <h2>Son Fotoğraflarım</h2>
            <div id="PhotosDiv">
                <ul id="PhotosUL">
                </ul>
            </div>
        </div>
        <div style="clear: both;">
        </div>
        <script type="text/javascript">
            function GetLastPhotos() {
                $("#PhotosUL").html("");
                $.ajax({
                    type: "GET",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    url: 'https://api.instagram.com/v1/users/' + userId + '/media/recent?access_token=' + accessToken,                    
                    dataType: "jsonp",
                    cache: false,
                    beforeSend: function () {
                        $("#loading").show();
                    },
                    success: function (data) {
                        $("#loading").hide();
                        if (data == "") {
                            $("#PhotosDiv").hide();
                        } else {

                            $("#PhotosDiv").show();
                            for (var i = 0; i < data["data"].length; i++) {
                                $("#PhotosUL").append("<li style='float:left;list-style:none;'><a target='_blank' href='" + data.data[i].link + "'><img src='" + data.data[i].images.thumbnail.url + "'></img></a></li>");
                            }
                        }
                    }
                });
            }
            $(document).ready(function () {
                GetLastPhotos();
            });
        </script>
    </form>
</body>
```

Sayfada yer alan Son Fotoğrafları Getir başlıklı buton'a basıldığında aslında önceden login olunmamışsa Instagram üzerinden bir giriş işlemi yaptırılır. Giriş işlemini takiben akış tekrardan Default.aspx sayfasına yönlenir (Instagram'a dönülürken nereye gidileceği bilgisini uygulamamızı register ederken söylemiştik) Eğer dönüş başarılı ise GetLastPhotos isimli javascript içeriği devreye girer. Bu fonksiyon içerisinde Instagram REST Endpoint'lerine Ajax servis çağrısı gerçekleştirilir. HTTP GET tipinden yapılan bu çağrıda içerik tipi (content-type) JSON olarak bildirilir. Ayrıca asenkron tipte bir çağrı yapılacağı da belirtilmiştir (async:true). url niteliğine verilen değerde ise talep edilen fonksiyonelliğe ait adres bilgisi yer alır. Tabii burada kullanılan userId ve accessToken değerlerinin, GetUserIdAndAccessToken metodu içerisinden verildiklerine dikkat edelim (O script'i bu yüzden sayfaya register ettik) Eğer işlemler başarılı bir şekilde gerçekleşirse success bloğundaki kod bloğu devreye girecek ve ilgili div söz konusu kullanıcının son fotoğrafları ile dolacaktır. Örneğin aşağıdaki gibi:)

![UsingInstagram_2.gif](/assets/images/2016/UsingInstagram_2.gif)

Görüldüğü gibi son fotoğraflarımız geldi. Hatta içlerinden istenilen bir tanesine tıklayabilir ve Instagram üzerindeki geçerli adresine gidebiliriz. url içeriğini değiştirerek farklı talepleri de test edebiliriz. Örneğin tag bazlı veya popüler olan fotoğrafları, sizi takip eden kullanıcıları, sizin takip ettiklerinizi, belli bir fotoğrafı ya da video'yu beğenenleri vb...Kısacası Instagram için yazılmış herhangibir uygulamanın yapabileceği pek çok şeyi REST Endpoint'leri kullanarak gerçekleştirmeniz mümkün.

Bu örnekte bir Instagram REST Endpoint'ini kullanarak son 25 fotoğrafımızı web sayfası üzerine nasıl basabileceğimizi gördük. Dikkat edilmesi gereken nokta çalışma prensibi. Öncelikle Instagram'dan uygulamamız için izin aldık. Bu izin karşılığında bize verdiği jetonu ve kendisine uygulamamız üzerinden giriş yaptığımızı kullanıcı bilgisine ele alarak sorgu işlemi gerçekleştirdik. Elde ettiğimiz JSON içeriklerini sayfa üzerinde uygun bir şekilde kullandık ve vakamızı tamamladık. Böylece geldik bir makalemizin daha sonuna. Tekrar görüşünceye dek hepinize mutlu günler dilerim.
