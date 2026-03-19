---
title: "Vue ve NW.js ile Desktop Uygulaması Geliştirmek"
pubDate: 2019-06-14 13:00:00
categories:
  - Vue.js
tags:
  - Vue.js
  - Bash
  - JavaScript
  - C#
  - .NET
  - LINQ
  - JSON
  - Web API
  - HTTP
  - Node.js
  - Vue
  - Generics
  - GitHub
---

# Vue ve NW.js ile Desktop Uygulaması Geliştirmek
![perfectstr.png](images/perfectstr.png)

Geçen gün fark ettim ki yaş ilerleyince blogumdaki yazıların girişinde kullanabileceğim malzeme sayısı da artmış. Söz gelimi şu anda lise son yıllarıma yani seksenlerin sonu doksanların başına doğru gitmiş durumdayım. O dönemlerde kısa Amerikan dizileri popüler. Hatta Arjantin menşeeli diziler de çok yaygın. Sanıyorum Mariana isimli popüler bir dizi vardı. Kısa boylu, siyah kıvırcık saçlı, buğday tenli ve hayatı acılar içinde geçen bir Latin kadının hikayesiydi. Lakin ben hayatı toz pembe görmemize vesile olan komedileri tercih ediyordum. Hatta en çok sevdiğim komedi dizisi [Perfect Strangers](https://www.imdb.com/title/tt0090501/?ref_=nv_sr_2?ref_=nv_sr_2)'dı.

Mipos isimli Yunan köyünden Chicago'daki kuzeni Larry Appleton'ın yanına yerleşip "Komik olma kuzen" repliği ile zihnime kazınan Balki Bartokomous bizleri epeyce güldürürdü. Aradan çeyrek asır geçmiş olsa da aptal kutunun bizleri ekrana bağlayan bazı alışkanlıkları değişmiyor. Platformlar belki ama yine komedi dizileri, yine Arjantin dizileri ve yine aklımıza kazınan Balki'ler var. [Saturday-Night-Works'ün 16 numaralı çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2016%20-%20Build%20Desktop%20App%20with%20Vue%20and%20NWjs)na konu olan Big Bang Theory'de işte bana bu çağrışımları yapmış durumda. Öyleyse gelin başlayalım.

Daha önceden [Electron ile cross platform desktop uygulamaları](https://github.com/buraksenyurt/electron)nın geliştirilmesi üzerine çalışmıştım (github repo istatistiklerine göre kimsenin ilgisini çekmemişti ama malum çok eski bir desktop programıcısı olduğumdan ilgilenmiştim) Bu kez eskiden node-webkit olarak bilinen [NW.js kullanarak](https://nwjs.io/) WestWorld üzerinde desktop uygulaması geliştirmek istedim. NW.js cephesinde de aynen Electron'da olduğu gibi Chromium, Node.js, HTML, CSS ve javascript kullanılmakta. Lakin ufak tefek farklılıklar var. Electron'da entry point yeri Javascript script'i iken NW.js tarafında script haricinde bir web sayfası da giriş noktası olabiliyor. Build süreçlerinde de bir takım farklılıklar var.

Peki bu çalışma kapsamında ne yapacağız? Uygulama çok basit bir arayüze sahip olacak. Ekrandaki metin kutusuna bir isim girilecek ve Big Bang Theory'nin ilgili bölümüne ait bazı bilgiler ekrana bastırılacak (Akıllı bir arama ekranı değil çok şey beklemeyin) Bölüm bilgisini ise bigbangapi isimli ve.net core ile yazılmış bir web api servisi sağlayacak.

## Başlangıç

WestWorld'de (Ubuntu 18.04 64bit) bu örnek için Vue CLI'a (Vue'nun Command Language Interface aracı olarak düşünebiliriz) ihtiyaç var. Önce versiyonu kontrol edip yoksa yüklemek lazım. Ayrıca projeyi oluşturduktan sonra NW paketini de eklemek gerekiyor. axios'u servis haberleşmesi için kullanacağız. Bunun için terminalden aşağıdaki adımlarla ilerleyebiliriz. vue create ile başlayan satır bbtheory isimli hazır bir Vue uygulaması inşa edecek. npm install satırlarında da bu uygulama için gerekli paketlerin yüklenmesi sağlanıyor. Nw sdk ve axios bu anlamda önemli.

```bash
vue --version
sudo npm install -g @vue/cli
vue create bbtheory
cd bbtheory
sudo npm install --save-dev nwjs-builder-phoenix nw@sdk
sudo npm install axios
```

> Vue projesi varsayılan kurulum ayarları ile oluşturulmuştur.

## Kod Tarafı

Gelelim kodlama tarafına. Uygulamanın masaüstü arayüzü olan App bileşeni app.vue dosyasında kodlanıyor. Bu dosyayı aşağıdaki gibi değiştirerek ilerleyebiliriz. Sonuçta HTML tabanlı bir ortam var. Elbette Vue'ya özgü bir sentaks da söz konusu. Söz gelimi bileşendeki bir kontrolü model tarafına bağlamak için v-model direktifinden yararlanılıyor. Bir section elementinin görünürlüğünü koşullandıracaksak v-if direktifini kullanabiliyoruz. Button kontrolündeki olayları betikteki bir fonksiyonla ilişkilendirirken @click şeklindeki element adı ele alınıyor. Modeldeki özellikleri kontrollerde gösterirkense {{propertyName}} notasyonuna başvuruyoruz.

Örneğimizdeki bileşen, önyüz tasarımı ve kodu aynı dosya içerisinde barındırmakta. Ancak hazır olarak gelen şablonu incelerseniz Components klasöründe bir bileşen geldiğini de görebilirsiniz. Yani alt bileşenleri bu klasör altında da toplayabiliriz. Bu arada kodlarda yakaladığınız yorum satırlarını okumayı unutmayın. Destekleyici bilgiler görebilirsiniz.

```text
<template>
  <div id="app">
    <h2>Bölüm adını yazar mısın?</h2>
    <section class="input-Section">
      <input type="text" v-model="query">
      <button :disabled="!query.length" @click="findEpisode">Göster</button>
      <!-- butona basılınca findEpisode metodu çağırılacak -->
    </section>
    <section v-if="error">
      <!-- error değişkeni true olarak set edilmişse bir şeyler ters gitmiştir -->
      <i>Sanırım bölüm bulunamadı ya da bir şeyler ters gitti</i>
    </section>
    <section v-if="!error">
      <!-- Aranan veri bulunduysa -->
      <h1>{{name}} ({{season}}/{{number}}) - {{ airdate }}</h1>
      <div><p>{{summary}}</p></div>
      <div>
        <img :src="imageLink"/>
      </div>
    </section>
  </div>
</template>

<script>
export default {
  name: "Pilot",
  data() {
    // data modelimiz api servisinden dönen tipe göre düzenlendi
    return {
      query: "",
      error: false,
      id: null,
      name: "",
      airdate:"",
      season: null,
      number: null,
      summary: "",
      imageLink: ""
    };
  },
  methods: {
    findEpisode() {
      // api servisine talep gönderen metod
      this.$http
        .get(`/episode/${this.query}`) // sorguyu tamamlıyoruz. parametre olarak input kontrolüne girilen değer alınıyor. query değişkeni üzerinden.
        .then(response => {
          this.error = false;
          this.name = response.data.name; // servisten gelen cevabın içindeki alanların, vue data modelindeki karşılıklarına ataması yapılıyor
          this.season = response.data.season;
          this.number = response.data.number;
          this.summary = response.data.summary; 
          this.airdate=response.data.airDate;
          this.imageLink=response.data.imageLink;
          console.log(response.data); //control amaçlı
        })
        .catch(() => {
          // hata alınması durumu
          this.error = true;
          this.name = "";
        });
    }
  }
};
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  padding:10px;
  text-align: center;
  color: #2c3e50;
  margin-top: 10px;
}
input {
  width: 75%;
  outline: none;
  height: 20px;
  font-size: 1em;
}

button{
  display: block;
  width: 25%;
  height: 25px;
  outline: none;
  border-radius: 4px;
  white-space: nowrap; 
  margin:0 10px;
  font-size: 1rem;
}

.input-Section {
  display: flex;
  align-items: center;
  padding: 20px 0;
}

</style>
```

App bileşeninde dikkat edileceği üzere $http ile yapılan bir servis çağrısı var. Bu axios tarafından sağlanacak bir hizmet. Bu nedenle main.js dosyasında gerekli hazırlıkların yapılması lazım. Dikkat edileceği üzere Vue çalışma zamanının axios'u $http özelliği üzerinden kullanabilmesini sağlayacak bir enjekte işlemi söz konusu.

```javascript
import Vue from 'vue'
import App from './App.vue'
import axios from 'axios' // API servisine HTTP talebini göndermek için kullandığımız modül

axios.defaults.baseURL = 'http://localhost:4001/api/'; // base url adresini atadık
Vue.http = Vue.prototype.$http = axios;
Vue.config.productionTip = false

new Vue({
  render: h => h(App),
}).$mount('#app')
```

Bu konu kapsamı dışında ancak.Net Core tabanlı bir Web API hizmetimiz de bulunuyor. Bu servis dizinin bölümlerini aramak amacıyla kodladığımız sahte bir program. Konumuzla doğrudan ilintili olmadığı için detayına girmemize gerek yok ama en azından Controller sınıfında neler yaptığımıza bir bakalım derim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace bigbangapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {

        [HttpGet("{name}")]
        public ActionResult<Episode> Get(string name)
        {
            try
            {
                string db = System.IO.File.ReadAllText("db/content.json");
                JObject json = JObject.Parse(db);
                JArray episodes = (JArray)json["episodes"];
                var all = episodes
                            .Select(e => new Episode
                            {
                                Id = (int)e["id"],
                                Name = (string)e["name"],
                                Season = (int)e["season"],
                                Number = (int)e["number"],
                                Summary = (string)e["summary"],
                                ImageLink = (string)e["image"]["medium"],
                                AirDate=(string)e["airdate"]
                            });
                var result = all.Where(e => e.Name == name).FirstOrDefault();
                return new ActionResult<Episode>(result);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
```

Örneğin basitliği açısından yalın bir Get operasyonu sunuyoruz. Parametre olarak gelen bölüm adını fiziki olarak tuttuğumuz content.json içeriğinde arayarak bir sonuç döndürmekteyiz. Pek tabii bu sahte bir servis. Veri kaynağı olarak fiziki dosya yerine veri tabanı kullanılan bir moda da geçebiliriz. Hatta film bilgileri sunan bir gerçek hayat API'sini de tercih edebiliriz. Tercih size kalmış.

Ah unutmadan! Geliştirme safhasında kuvvetle muhtemel CORS (Cross Origin Resource Sharing) ile ilgili bir sorun yaşayabilirsiniz. Bu nedenle Startup.cs içerisinde CORS özelliğini etkinleştirmemiz ve masaüstünden gelecek cevapları kabul edebileceğimizi belirtmemiz gerekiyor.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
	// Diğer uygulamanın node.js servisinin buraya axios üzerinden
	// talep atabilmesi için Cors desteği eklenmiştir
	// Configure metodu içerisinde de 8080 kaynağından gelecek
	// tüm metodlar için izin yetkisi bildirilmiştir.
	services.AddCors();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseCors(
		options=>options.WithOrigins("http://localhost:8080").AllowAnyMethod()
	);
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}
	else
	{
		app.UseHsts();
	}

	//app.UseHttpsRedirection();
	app.UseMvc();
}
```

Tekrar Vue tarafına dönerek ilerleyelim. Uygulamanın giriş noktasını belirtmek için package.json dosyasına main özelliğini eklememiz ve bir adres yönlendirmesi yapmamız gerekiyor. Bu sayede uygulama kodunda yapılan her değişiklik anında çalışma zamanına da yansıyacaktır (Program çalıştıktan sonra önyüz bileşeni olan App.vue dosyasında değişiklikler yapmayı deneyin)

```javascript
"main": "http://localhost:8080",
```

## Çalışma Zamanı

Normalde desktop uygulamasını çalıştırmak için proje klasöründeyken birinci terminalden

```bash
npm run serve
```

ile sunucuyu etkinleştirmek ve ardından ikinci bir terminal penceresinden

```bash
./node_modules/.bin/run .
```

yazmak gerekiyor. Lakin bu durumda NW.js'in ilgili SDK'sı indirilip development ortamı ayağa kalkıyor. Bunu otomatikleştirmek için nw@sdk isimli paketi yüklemek ve package.json dosyasındaki script bölümüne örneğin desktop isimli yeni bir çalışma zamanı parametresi dahil etmemiz yeterli.

```javascript
  "scripts": {
    "serve": "vue-cli-service serve",
    "build": "vue-cli-service build",
    "lint": "vue-cli-service lint",
    "desktop": "nw ."
  },
```

Desktop uygulaması çalıştıktan sonra tarayıcının Development Tools'unu kullanarak debug yapılması mümkün. Masaüstü tarafından yapılan API çağrılarını ve dönen sonuçları buradan izleme şansımız var. Tabii tüm bunların başında yazdığımız web api servisinin de çalışır durumda olması gerekiyor öyle değil mi? Sonrasında Node.js server ve desktop uygulaması çalıştırılarak ilerlersek yerinde olacaktır. Bunları üç ayrı terminal penceresinden yürütebiliriz ama temel olarak aşağıdaki komutları kullanmamız lazım.

```bash
dotnet run
npm run serve
npm run desktop
```

Eğer bir sorun olmazsa uygulama ayağa kalktıktan sonra Big Bang Theory'den örnek bir bölümü aratabiliriz. Ben aşağıdaki gibi bir sonuca ulaşmışım.

![05_16_cover_1.png](images/05_16_cover_1.png)

## Paketleme

Uygulamayı paketlemek çok daha mantıklı ve gerekli elbette. Sonuçta dağıtımını (Deployment) yapmak isteyeceğiz. Bunun için packages.json içerisine build bölümünü aşağıdaki gibi eklememiz lazım.

```javascript
  "build": {
    "nwVersion": "0.35.5"
  }
```

Dikkat edileceği üzere nw paketinin hangi versiyonunu kullanacağımızı belirtiyoruz (Güncel sürümüne bakmanızda yarar var) bbtheory isimli uygulamanın root klasöründe aşağıdaki komut ile 64bit linux platformu için gerekli paketin üretilmesi sağlanabiliyor.

```bash
./node_modules/.bin/build --tasks linux-x64 .
```

![05_16_cover_2.png](images/05_16_cover_2.png)

Paket boyutu oldukça yüksek görüldüğü üzere! Zaten cross-platform masaüstü uygulamaları için en rahatsız edici konuların başında da dosya boyutları geliyor. Ancak küçültmek için çeşitli yollar olduğu ifade edilmekte. Bunu henüz araştırma fırsatım olmadı ancak [yakın tarihli şu yazıda bir takım bilgiler](https://dev.to/thejaredwilcurt/reducing-app-distribution-size-in-nwjs-3d5f) mevcut.

![05_16_cover_3.png](images/05_16_cover_3.png)

## Ben Neler Öğrendim?

Elbette aptal kutunun başında saatlerimi geçirdiğim Perfect Strangers dizisinin bana alttan alttan verdiği mesajlar gibi bu örnek çalışma sonrasında öğrendiğim bazı şeyler de olmadı değil. Bunları aşağıdaki gibi özetlemeye çalışayım.

- Vue tarafında ön yüz nasıl geliştirilir
- v-model, v-if, {{ }}, @click gibi Vue ilişkili ifadeler ne işe yarar
- Bileşen ile model özellikleri nasıl kullanılır
- axios ile node.js tarafından servis talepleri nasıl gönderilir
- newtonsoft.json ile bir json dizisinde nasıl linq sorgusu çalıştırılır
- CORS ne işe yarar

Ne yazık ki Vue konusunda uzman değilim. Aslında onu şirketteki yeni nesil projelerde kullanıyoruz lakin iyi bir başlangıcım yok. Belki de ahch-to (macOS High Sierra) üzerinde yapacağım ikinci faz çalışmaları kapsamında ona daha fazla zaman ayırabilirim. Böylece geldik neşeli [bir cumartesi gecesinin 16ncı bölümüne ait derlemeler](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2016%20-%20Build%20Desktop%20App%20with%20Vue%20and%20NWjs)in de sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
