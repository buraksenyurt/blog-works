---
title: "Hasura GraphQL Engine ile geliştirilmiş bir API Servisini Vue.js ile Kullanmak"
pubDate: 2019-11-06 10:30:00
categories:
  - Vue.js
tags:
  - Vue.js
  - JavaScript
  - Bash
  - PostgreSQL
  - HTTP
  - JWT
  - Authentication
  - Docker
  - Kubernetes
  - Vue
  - Performance
  - Caching
  - GitHub
---

# Hasura GraphQL Engine ile geliştirilmiş bir API Servisini Vue.js ile Kullanmak
![hasuralogo.png](images/hasuralogo.png)

Yıl 2015. Hindistan'ın Bengaluru şehrinde doğan bir Startup (Sonradan San Fransico'da da bir ofis sahibi olacaklar), [Microsoft'un BizSpark programından destek](https://blogs.technet.microsoft.com/bizspark_featured_startups/2017/09/18/quickly-develop-backend-applications-without-having-to-write-code-with-hasura/) buluyor. Kurucuları Rajoshi Ghosh (Aslen bioinformatik araştırmacısı) ve Tanmai Gopal (Bulut sistemleri, fonksiyonel programlama ve GraphQL konusunda uzman) isimli iki Hintli. Şirketlerine şeytanın sanskritçedeki adını veriyorlar; Hasura! Aslında O, fonksiyonel dillerin kralı Haskell ile yazılmış bir platform ve şimdilerde Heroku ile daha yakın arkadaş.

Ekibin amacı geliştiricilerin hayatını kolaylaştıracak, yüksek hızlı, kolayca ölçeklenebilir, sade ve Kubernetes ile dost PaaS (Platform as a Service) ile BaaS (Back-end as a Service) ortamları sunmak.

İsimlendirmenin gerçek hikayesi tam olarak nedir bilemiyorum ama eğlenceli bir logoları olduğu kesin:) Startup'ların en sevdiğim yanlarından birisi de bu. Özgün, tabulara takılmadan, etki bırakacak şekilde düşünülen isimleri, renkleri, logoları...Belki de arka planda sessiz sedasız süreçler çalıştıran back-end servislerini birer iblis olarak düşündüklerinden bu ismi kullanmışlardır. Hatta Heroku üzerinde koştuğunu öğrenince Japonca bir kelime olduğunu bile düşünmüştüm. Hu novs?! Ama işin özü verdikleri önemli hizmetler olduğu. Bunlardan birisi de GraphQL motorları.

API'ler için türlendirilmiş (typed) sorgulama dillerinden birisi olarak öne çıkan GraphQL'e bir süredir uğramıyordum. Daha doğrusu GraphQL sorgusu çalıştırılabilecek şekilde API servis hazırlıklarını yapmaya üşeniyordum. Bu nedenle işi kolaylaştıran ve Heroku üzerinden sunulan [Hasura GraphQL Engine](https://hasura.io/) hizmetine bakmaya karar vermiştim. Hasura, veriyi PostgreSQL kullanarak saklıyor ve ayrıca API'yi bir Docker Container içerisinden sunuyor. Amacım Hasura tarafında hazırlayacağım iki kobay veri setini, Vue.js tabanlı bir istemcisinden tüketmekti. Basitçe listeleme ve veri ekleme işlerini yapabilsem başlangıç için yeterli olacaktı. Öyleyse ne duruyoruz. [37nci saturday-night-works çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2037%20-%20Hasura%20with%20Vue)nın derlemesine başlayalım. İlk olarak Hasura servisini hazırlayacağız.

## Hasura GraphQL Engine Tarafının Geliştirilmesi

Pek tabii Heroku üzerinde bir hesabımızın olması gerekiyor. Sonrasında [şu adrese](https://elements.heroku.com/) gidip elements kısmından Hasura GraphQL Engine'i seçmek yeterli.

![09_37_credit_1.png](images/09_37_credit_1.png)

Gelinen yerden Deploy to Heroku diyerek projeyi oluşturabiliriz.

![09_37_credit_2.png](images/09_37_credit_2.png)

Ben aşağıdaki bilgileri kullanarak bir proje oluşturdum.

![09_37_credit_3.png](images/09_37_credit_3.png)

Deploy başarılı bir şekilde tamamlandıktan sonra,

![09_37_credit_4.png](images/09_37_credit_4.png)

View seçeneği ile yönetim paneline geçebiliriz.

![09_37_credit_5.png](images/09_37_credit_5.png)

Dikkat edileceği üzere GraphQL sorgularını çalıştırabileceğimiz bir arayüz otomatik olarak sunuluyor. Ancak öncesinde örnek veri setleri hazırlamalıyız. Bunun için Data sekmesinden yararlanabiliriz.

![09_37_credit_6.png](images/09_37_credit_6.png)

Arabirimin kullanımı oldukça kolay. Ben aşağıdaki özelliklere sahip tabloları oluşturdum.

![09_37_credit_7.png](images/09_37_credit_7.png)

categories isimli tablomuzda unique tipte, get_random_uuid () fonksiyonu ile eklenen satır için rastgele üretilen categoryId ve text tipinden title isimli alanlar bulunuyor. categoryId, aynı zamanda primary key türünden bir alan.

![09_37_credit_8.png](images/09_37_credit_8.png)

products tablosunda da UUID tipinden productId, text tipinden description, number tipinden listPrice ve yine UUID tipinden categoryId isimli alanlar mevcut. categoryId alanını, ürünleri kategoriye bağlamak için (foreign key relations) kullanıyoruz. Ama bu alanı foreign key yapmak için Modify penceresine geçmeliyiz.

![09_37_credit_9.png](images/09_37_credit_9.png)

![09_37_credit_10.png](images/09_37_credit_10.png)

İlişkinin geçerlilik kazanması içinse, categories tablosunun Relationships penceresine gidip önerilen bağlantıyı eklemek gerekiyor.

![09_37_credit_14.png](images/09_37_credit_14.png)

![credit_15.png](images/credit_15.png)

![credit_16.png](images/credit_16.png)

> Bu durumda categories üzerinden products'a gidebiliriz. Ters ilişkiyi de kurabiliriz ve bir ürünle birlikte bağlı olduğu kategorinin bilgisini de yansıtabiliriz ki ürünleri çektiğimizde hangi kategoride olduğunu da göstermek güzel olur. Bunu nasıl yapabileceğinizi bir deneyin isterim.

Hasura'nın Postgresql tarafındaki örnek tablolarımız hazır. İstersek Insert Row penceresinden tablolara örnek veri girişleri yapabilir ve GraphiQL pencresinden sorgular çalıştırabiliriz. Ben yaptığım denemelerle alakalı bir kaç örnek ekran görüntüsü paylaşayım. Arabirimin sağ tarafında yer alan Docs menüsüne de bakabilirsiniz. Burada query ve mutation örnekleri, hazırladığımız veri setleri için otomatik olarak oluşturuluyorlar.

![09_37_credit_11.png](images/09_37_credit_11.png)

![09_37_credit_12.png](images/09_37_credit_12.png)

## Örnek Sorgular

Veri setimizi oluşturduktan sonra arabirim üzerinden bazı GraphQL sorgularını deneyebiliriz. Ben aşağıdaki örnekleri denedim.

Kategorilerin başlıklarını almak.

```javascript
query{
  categories{
    title
  }
}
```

![credit_19.png](images/credit_19.png)

Kategorilere bağlı ürünleri çekmek.

```javascript
query{
  categories{
    title
    products{
      description
      listPrice
    }
  }
}
```

![credit_18.png](images/credit_18.png)

Ürünlerin tam listesi ve bağlı olduğu kategori adlarını çekmek.

```javascript
query{
  products{
    description
    listPrice
    category{
      title
    }
  }
}
```

![credit_17.png](images/credit_17.png)

Listeleme işlemleri dışında veri girişi de yapabiliriz. Bunun için mutation kullanıldığını daha önceden öğrenmiştim. Örneğin yeni bir kategoriyi aşağıdaki gibi ekleyebiliriz.

```javascript
mutation {
  insert_categories(objects: [{
    title: "Çorap",
  }]) {
    returning {
      categoryId
    }
  }
```

![credit_20.png](images/credit_20.png)

Hasura, GraphQL API’si arkasında PostgreSQL veri tabanını kullanırken SQLden aşina olduğumuz bir çok sorgulama metodunu da hazır olarak sunar. Örneğin fiyatı 300 birimin üstünde olan ürünleri aşağıdaki sorgu ile çekebiliriz.

```javascript
{
  products(where: {listPrice: {_gt: 300}}) {
    description
    listPrice
    category {
      title
    }
  }
}
```

![credit_21.png](images/credit_21.png)

Where metodu sorgu şemasına otomatik olarak eklenmiştir. _gt tahmin edileceği üzere greater than anlamındadır. Yukarıdaki sorguya fiyata göre tersten sıralama opsiyonunu da koyabiliriz. Sadece where koşulu arkasından order_by çağrısı yapmamız yeterlidir.

```javascript
{
  products(where: {listPrice: {_gt: 300}}, order_by: {listPrice: desc}) {
    description
    listPrice
    category {
      title
    }
  }
}
```

![credit_22.png](images/credit_22.png)

Çok büyük veri setleri düşünüldüğünde ön yüzler için sayfalama önemlidir. Bunun için limit ve offset değerlerini kullanabiliriz. Örneğin 5nci üründen itibaren 5 ürünün getirilmesi için aşağıdaki sorgu kullanılabilir.

```javascript
{
  products(limit: 5, offset: 5) {
    description
    listPrice
    category {
      title
    }
  }
}
```

![credit_23.png](images/credit_23.png)

Hasura Query Engine’in sorgu seçenekleri ile ilgili olarak [buradaki dokümanı](https://docs.hasura.io/1.0/graphql/manual/queries/index.html) takip edebilirsiniz.

## İstemci (Vue) Tarafı

Gelelim ilgili servisi tüketecek olan istemci uygulamamıza. İstemci tarafını basit bir Vue projesi olarak geliştirmeye karar vermiştim. Aşağıdaki terminal komutunu kullanıp varsayılan ayarları ile projeyi oluşturabiliriz. Ayrıca GraphQL tarafı ile konuşabilmek için gerekli npm paketlerini de yüklememiz gerekiyor. Apollo (ilerleyen ünitelerde ondan bir GraphQL Server yazmayı denemiştim), GraphQL servisimiz ile kolay bir şekilde iletişim kurmamızı sağlayacak. Görsel taraf içinse bootstrap kullanabiliriz.

```bash
sudo vue create nba-client
sudo npm install vue-apollo apollo-client apollo-cache-inmemory apollo-link-http graphql-tag graphql bootstrap --save
```

## Kod Tarafı

Vue uygulaması tarafında yapacaklarımız kabaca şöyle (Kod dosyalarındaki yorum bloklarında daha detaylı bilgiler mevcut)

Components klasörüne tek ürün için kullanılabilecek ProductItem isimli bir bileşen ekliyoruz. Anasayfa listelemesinde tekrarlanacak türden bir bileşen olacak bu. Bileşende product özelliği üzerinden içerideki elementlere veri bağlama işlemini gerçekleştirmekteyiz. {{nesne.özellik}} notasyonlarının nasıl kullanıldığına dikkat edelim.

```text
<template>
  <div :key="product.productId" class="card w-75">
    <div class="card-header">
      <p class="card-text">{{product.description}}</p>
    </div>
    <div class="card-body text-left">
      <h6 class="card-subtitle mb-2 text-muted">{{product.listPrice}} Lira</h6>
      <h6 class="card-subtitle mb-2">'{{product.category.title}}' kategorisinden</h6>
    </div>
    <div class="card-footer text-right">
      <a href="#" class="btn btn-primary">Sepete Ekle</a>
    </div>
      <hr/>
  </div>
</template>

<script>
export default {
  name: "ProductItem",
  props: ["product"]
};
</script>
```

Ürünlerin listesini gösterebilmek içinse ProductList isimli bir bileşen kullanacağız. Bunu da components altında aşağıdaki gibi yazabiliriz.

```text
<template>
  <div>
    <!--
      products dizisindeki her bir ürün için product-item öğesi ekliyoruz.
      Bu öğe bir ProductItem bileşeni esasında
      -->
    <product-item v-for="product in products" :key="product.productId" :product="product"></product-item>
  </div>
</template>

<script>
/*
  div içerisinde kullandığımız product-item elementi için ProductItem bileşenini eklememi gerekiyor.
  gql ise GraphQL sorgularını çalıştırabilmemiz için gerekli
*/
import ProductItem from "./ProductItem";
import gql from "graphql-tag";

/*
  GraphQL sorgumuz.
  Tüm ürün listeini, kategori adları ile birlikte getirecek
*/
const selectAllProducts = gql`
  query getProducts{
  products{
    productId
    description
    listPrice
    category{
      title
    }
  }
}
`;

/*
  Sorguyu GraphQL API'sine gönderebilmek için apollo'ya ihtiyacımız var.
  products dizisini query parametresine verilen değişken ile çekiyoruz.
*/
export default {
  name: "ProductList",
  components: { ProductItem }, // Sayfada bu bileşeni kullandığımız için eklendi
  data() {
    return {
      products: []
    };
  },
  apollo: {
    products: {
      query: selectAllProducts
    }
  }
};
</script>
```

Ürün ekleme işini ProductAdd isimli bileşen üstleniyor. Yine components sekmesinde konuşlandıracağımız tipin kod içeriği aşağıdaki gibi olmalı.

```text
<template>
  <!-- Veri girişi için basit bir formumuz var. Input değerlerini v-model niteliklerine verilen isimlerle bileşene bağlıyoruz -->
  <form @submit="submit">
    <fieldset>
      <div class="form-group w-75">
        <input
          class="form-control"
          aria-describedby="descriptionHelp"
          type="text"
          placeholder="Ürün bilgisi"
          v-model="description"
        >
        <small
          id="descriptionHelp"
          class="form-text text-muted"
        >Satışı olan basketbol malzemesi hakkında kısa bir bilgi...</small>
      </div>
      <div class="form-group w-75">
        <input class="form-control" type="number" v-model="listPrice">
        <small id="listPriceHelp" class="form-text text-muted">Ürünün mağaza satış fiyatı</small>
      </div>
      <div class="form-group w-75">
        <input
          class="form-control"
          type="text"
          placeholder="Halledene kadar kategorinin UUID bilgisi :D"
          v-model="categoryId"
        >
      </div>
      <!-- Kategoriyi drop down olarak nasıl ekleyebiliriz? -->
    </fieldset>
    <div class="form-group w-75 text-right">
      <button class="btn btn-success" type="submit">Dükkana Yolla</button>
    </div>
  </form>
</template>

<script>
import gql from "graphql-tag";
//import { InMemoryCache } from "apollo-cache-inmemory";

/*
  Bu veri girişi yapmak için kullanacağımız mutation sorgumuz.
  insert_products'u Hasura tarafında kullanmıştık hatırlarsanız.

  mutation parametrelerini belirlerken veri türlerine dikkat etmemiz lazım.
  Söz gelimi listPrice, Hasura tarafında Numeric tanımlandı. CategoryId değeri
  ise UUID formatında. Buna göre case-sensitive olarak veri tiplerini söylüyoruz.
  Aslında bunu anlamak için numeric! yerine Numeric! yazıp deneyin. HTTP 400
  Bad Request alıyor olmalısınız.
*/
const addNewProduct = gql`
  mutation addProduct(
    $description: String!
    $listPrice: numeric!
    $categoryId: uuid!
  ) {
    insert_products(
      objects: [
        {
          description: $description
          listPrice: $listPrice
          categoryId: $categoryId
        }
      ]
    ) {
      returning {
        productId
      }
    }
  }
`;

export default {
  name: "ProductAdd",
  data() {
    return {
      description: "",
      listPrice: 0,
      categoryId: ""
    };
  },
  apollo: {},
  methods: {
    /*
    form submit edildiği zaman devreye giren metodumuz.
    $data ile formdaki veri içeriğini description, listPrice ve categoryId olarak yakalıyoruz
    */
    submit(e) {
      e.preventDefault();
      const { description, listPrice, categoryId } = this.$data;

      /*
      apollo'nun mutate metodu ile addNewProduct isimli mutation sorgusunu çalıştırıyoruz.
      Sorgunun beklediği değişkenler this.$data ile zaten yakalanmışlardı.
      */
      this.$apollo.mutate({
        mutation: addNewProduct,
        variables: {
          description,
          listPrice,
          categoryId
        },
        refetchQueries: ["ProductList"] // Insert işlemini takiben ürün lstesini tekrardan talep ediyoruz
      });
    }
  }
};
</script>
```

Uygulamanın ana bileşeni olan App.Vue'da product-add ve product-list isimli nesnelerimizi aşağıdaki gibi yerleştirebiliriz.

```text
<template>
  <div id="app">
    <h2 class="text-left">Yeni Ürün</h2>
    <!-- Bileşenleri altalta dizdik -->
    <product-add/>
    <h2 class="text-left">Basketbol Ürünleri</h2>
    <product-list/>
  </div>
</template>

<script>
/*
  Ana bileşen içerisinde kullanılan alt bileşenlerin import edilmesi
*/
import ProductList from "./components/ProductList.vue";
import ProductAdd from "./components/ProductAdd.vue";

export default {
  name: "app",
  components: {
    ProductList,
    ProductAdd
  }
};
</script>
```

Main.js içerisinde de önemli kodlamalarımız var. Amaç Hasura'yı ve GraphQL'i kullanabilir hale getirmek. Kodlarını aşağıdaki gibi geliştirebiliriz.

```javascript
import Vue from 'vue';
import App from './App.vue';
import { ApolloClient } from 'apollo-client';
import { HttpLink } from 'apollo-link-http';
import { InMemoryCache } from 'apollo-cache-inmemory';
import 'bootstrap/dist/css/bootstrap.min.css';
import VueApollo from 'vue-apollo';

Vue.config.productionTip = false;

// Hasura GraphQL Api adresimiz
const hasuraLink = new HttpLink({ uri: 'https://basketin-cepte.herokuapp.com/v1alpha1/graphql' });

/* 
  Servis iletişimini sağlayan nesne
  GraphQL istemcileri veriyi genellikle cache'de tutar.
  Tarayıcı ilk olarak cache'ten okuma yapar. 
  Performans ve network trafiğini azaltmış oluruz bu şekilde.
*/
const apolloClient = new ApolloClient({
  link: hasuraLink, // Kullanacağı servis adresini veriyoruz
  connectToDevTools: true, // Chrome'da dev tools üzerinde Apollo tab'ının çıkmasını sağlar. Debug işlerimiz kolaylaşır
  cache: new InMemoryCache() // ApolloClient'ın varsayılan Cache uyarlaması için InMemoryCache kullanılıyor. 
});

// Vue ortamının GraphQL ile entegre edebilmek için VueApollo kütüphanesini entegre ediyoruz. (https://akryum.github.io/vue-apollo/)
Vue.use(VueApollo);

/* 
  Vue tarafında GraphQL sorguları oluşturabilmek ve veri girişleri(mutations)
  yapabilmek için ApolloProvider örneği kullanmamız gerekiyor.
  VueApollo'den üretilen bu nesnenin Hasura tarafına işlemleri commit
  edebilmesi içinse yukarıdaki apolloClient'ı parametre olarak atıyoruz
*/
const apolloProvider = new VueApollo({
  defaultClient: apolloClient
});

new Vue({
  apolloProvider,// Vue uygulamamızın ApolloProvider'ı kullanabilmesi için eklendi
  render: h => h(App),
}).$mount('#app');
```

> TODO (Benim tembelliğimden size düşen)
> Bu servisi JWT Authentication bünyesine almak lazım. İşte size güzel bir araştırma konusu. Başlangıç noktası olarak Auth0'ın [şu dokümanına](https://auth0.com/docs/quickstart/spa/vuejs) bakılabilir. Ben şu an için sadece HASURA_GRAPHQL_ADMIN_SECRET kullanarak servis adresine erişimi kısıtlamış durumdayım. Zaten büyük ihtimalle yazıyı okuduğunuzda onun yerinde yeller estiğine şahit olacaksınız.

## Çalışma Zamanı

Hasura servisimiz ve istemci taraftaki uygulamamız hazır. Artık çalışma zamanına geçip sonuçları irdeleyebiliriz. Programı başlatmak için

```bash
npm run serve
```

terminal komutunu vermemiz yeterli. Sonrasında http://localhost:8080 adresine giderek ana sayfaya ulaşabiliriz. Aynen aşağıdakine benzer bir görüntü elde etmemiz gerekiyor.

![credit_24.png](images/credit_24.png)

Yeni ürün ekleme bileşeni konulduktan sonrasına ait örnek bir ekran görüntüsünü de buraya iliştirelim.

![credit_25.png](images/credit_25.png)

Hatta yeni bir forma eklediğimizde gönderilen Graphql Mutation sorgusundan dönen değer, F12 sonrası Network sekmesinden yakalayabiliriz.

![credit_26.png](images/credit_26.png)

> throw new UnDoneException ("Yeni ürün ekleme sayfasında kategori seçiminde combobox kullanımı yapılmalı");

## Ben Neler Öğrendim?

Doğruyu söylemek gerekirse bu çalışma benim için oldukça keyifliydi. Heroku platformunu oldukça beğeniyorum. Şirkette Vue tabanlı ürünlerimiz var ama onlar üzerinden çok iyi değilim. Dolayısıyla Vue tarafında bir şeyler yapmış olmak beni mutlu ediyor. Peki bu çalışma kapsamında neler mi öğrendim. İşte listem...

- Heroku'da Docker Container içerisinde çalışan ve PostgreSQL verilerini GraphQL ile sorgulanabilir olarak sunan Hasura isimli bir motor olduğunu
- Hasura arabirimden tablolar arası ilişkileri nasıl kuracağımı
- Bir kaç basit GraphQL sorgusunu (sorgularda sayfalama yapmak, ilişkili veri getirmek, where koşulları kullanmak)
- Vue tarafında GraphQL sorgularının nasıl gönderilebileceğini
- Component içinde Component kullanımlarını (App bileşeni dışında product-list içinde product-item kullanımı)
- Temel component tasarlama adımlarını
- Vue tarafından bir Mutation sorgusunun nasıl gönderilebileceğini ve schema veri tiplerine dikkat etmem gerektiğini

Böylece geldik bir [cumartesi gecesi derlemesi](https://github.com/buraksenyurt/saturday-night-works)nin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
