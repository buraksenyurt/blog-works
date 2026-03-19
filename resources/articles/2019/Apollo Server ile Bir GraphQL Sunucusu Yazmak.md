---
title: "Apollo Server ile Bir GraphQL Sunucusu Yazmak"
pubDate: 2019-04-26 07:07:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JavaScript
  - PostgreSQL
  - MongoDB
  - NoSQL
  - REST
  - JSON
  - HTTP
  - Async/Await
  - Dependency Injection
  - Visual Studio
  - GitHub
  - Dependency Management
---

# Apollo Server ile Bir GraphQL Sunucusu Yazmak
![apollo13.jpg](images/apollo13.jpg)

James A. Lovell, John L. Swigert, ve Fred W. Haise. Bu isimleri düşününce belki de çoğumuzun aklına bir şey gelmiyordur. Peki ya, Amerikalı veya İngiliz oldukları düşünülen bu şahısların yerine şu isimleri söylersek. Tom Hanks, Bill Paxton ve Kevin Bacon. Hımm...Sanırım birilerinin zihninde bir şeyler canlandı. Evet, evet...Bunlar film yıldızları değil mi? Üçü bir arada hangi filmde oynamışlardı acaba? Hala anımsayamadıysanız işte bir ipucu daha. "Houston we've got a problem." Şimdi anımsadınız mı?

Başta söylediğimiz isimler, 11 Nisan 1970 tarihinde uzaya fırlatılan Apollo 13 mürettebatına ait. Apollo programınındaki bu uçuşun amacı aya insan götürmekti. Ne yazık ki mekik, uçuşunun ikinci gününde meydana gelen bir kaza sonrası acil olarak dünyaya dönüş yapmak zorunda kalmıştı. Yazılanlardan okuduğumuz ve filmden gördüğümüz kadarıyla astronotlar çok zor koşullara göğüs gererek mucizevi bir dönüş hikayesinin altına imza atmışlardı. Tesadüf bu ya, geçenlerde filmini tekrardan izlediğim gün Apollo isimli Framework GraphQL arayüzü ile cumartesi gecesi çalışmalarımı yürütüyordum. Örneği orada tamamladıktan uzun süre sonra sağını solunu biraz derleyip bloğuma kendime not olarak düşeyim dedim. Haydi başlayalım.

Bildiğiniz üzere Facebook menşeili GraphQL son yılların yükselen trendlerinden. Çalışmakta olduğum şirket dahil bir çok yerde mikro servisler söz konusu olduğunda REST API mi GraphQL mi sıklıkla karşılaştırılıyor. Ben henüz emekle aşamasında olduğum için GraphQL'i anlamaya çalıştığım bir dönemdeyim. Basit örnekler dışında bu seferki amacımsa stand alone olarak çalışabilen bir GraphQL sunucusu yazmak. Bu amaçla tavsiye edilen Apollo Server API arayüzünü kullanmaya karar verdim. Bu arada [Apollo](https://www.apollographql.com/) uzun zamandır kabul görmüş bir[Framework olarak Thoughtworks teknoloji radarının](https://www.thoughtworks.com/radar/languages-and-frameworks/apollo) merceğinde yer alıyor. 2018 Nisan ayında Trial, 2019 Mayısında ise Adopt kategorsinde değerlendiriliyor.

![twgql.png](images/twgql.png)

Apollo Server program arayüzü web, mobile gibi istemciler için GraphQL servisi sunan bir ürün olarak düşünülebilir. Otomatik API doküman desteği sunar ve herhangibir veri kaynağını kullanabilir. Yani bir veri tabanını veya bir mikroservisi ya da bir REST APIyi, GraphQL hizmeti verecek şekilde istemcilere açabilir. Tek başına sunucu gibi çalıştırılabilmektedir. Pek tabii Heroku gibi cloud ortamlar üzerinde Serverless modda da kullanılabilmekte. Takip ettiğim Apollo Server dokümanlarındaki çalışma modelini bende aşağıdaki gibi resmetmeye çalıştım.

![04_38_credit_1.png](images/04_38_credit_1.png)

İstemciler kendilerine uygun Apollo Client paketlerini kullanarak sunucu tarafı ile kolayca haberleşebilirler. Benim bu çalışmadaki amacım stand alone çalışan bir Apollo sunucusu yazmak ve arka tarafta bir veri tabanını kullanarak (muhtemelen PostgreSQL) veriyi GraphQL üzerinden istemcilere açmak.

## Başlangıç

Proje iskeletini aşağıdaki gibi oluşturabilir ve Node.js tarafı için gerekli paketleri yükleyebiliriz (Örneği her zaman olduğu gibi WestWorld-Ubuntu 18.04, 64bit- üzerinde denemekteyim)

```bash
mkdir project-server
cd project-server
npm init
npm install apollo-server graphql
touch server.js
```

Kodları node.js tarafında geliştireceğiz. Bu nedenle npm init ile işe başlıyoruz. Ardından gerekli paketleri yükleyip, server.js isimli dosyamızı oluşturuyoruz. Ben örnek kodları Visual Studio Code ile geliştiriyorum.

## Birinci Sürüm (Dizi Kullanılan)

İlk sürümde veriyi bir diziyle beslemeye çalışacağız. İlk amacımız Apollo Server'ı ayağa kaldırabilmek. Kodları dikkatlice okumanızı öneririm. Gerekli açıklamalarla desteklemeye çalıştım.

```javascript
const { ApolloServer, gql } = require('apollo-server');

const tasks = []; // İlk denemeler için veri kümesini dummy array olarak tasarlayabiliriz

/*
    Tip tanımlamalarını yaptığımız bu kısım iki önemli parçadan oluşuyor.

    Queries: istemciye sunduğumuz sorgu modelleri
    Schema : veri modelini belirlediğimiz parçalar (Task gibi)

    Task isimli bir veri modelimiz var.
    Ayrıca sundacağımız sorgu modellerini de Query tipinde belirtiyoruz.
    AllTasks tüm task içeriklerini geri döndürürken, TaskById ile Id bazlı olarak
    tek bir Task dönecek.

    Veri manipülasyonu için InputTask modeli tanımlanmış durumda.
    Bu modeli Create, Update, Delete işlemlerine ait Mutation tanımında kullanıyoruz.

    Int değişkeninin Task tipinin tanımlanması dışındaki yerlerde ! ile yazıldığına dikkat edelim.
*/
const typeDefs = gql`
    # Entity modelimiz olarak düşünebiliriz
    type Task{
        id:Int
        title:String
        description:String
        size:String
    }

    # Silme operasyonundan deneme mahiyetinde farklı bir tip döndük
    type DeleteResult{
        DeletedId:Int,
        Result:String
    }
    # Sunduğumuz sorgular
    type Query{
        AllTasks:[Task]
        TaskById(id:Int!): Task
    }
    # Insert ve Update operasyonlarında kullanacağımzı model
    input TaskInput {
        id:Int!
        title:String
        description:String
        size:String
    }
    # CUD operasyonlarına ait tanımlamalar
    # Burada kullanılan parametre adları, Mutation tarafında da aynen kullanılmalıdır
    type Mutation{
        Insert(payload:TaskInput) : Task
        Update(payload:TaskInput):Task
        Delete(id:Int!):DeleteResult
    }
`;

/*
    Asıl verini ele alındığı çözücü tanımı olarak düşünülebilir.
    CRUD operasyonlarının temel işleyişinin yer aldığı, iş kurallarının da
    konulabildiği kısımdır.
    İki alt parçadan oluşmakta. Select tarzı sorgular için bir kısım (Query)
    ve CUD operasyonları için diğer bir kısım (Mutation)
    Şimdilik Array kullanıyoruz ama bunu MongoDB'ye çekmek isterim.
*/
const resolvers = {
    Query: {
        AllTasks: () => tasks,
        TaskById: (root, { id }) => {
            return tasks.filter(t => {
                return t.id === id;
            })[0];
        }
    },
    Mutation: {
        Insert: (root, { payload }) => { // Yeni veri ekleme operasyonu
            //console.log(payload);
            tasks.push(payload);
            return payload;
        },
        Update: (root, { payload }) => { // Güncelleme operasyonu
            // Gelen payload içindeki id değerini kullanarak dizi indisini bul
            var index = tasks.findIndex(t => t.id === payload.id);
            // alanları gelen içerikle güncelle
            tasks[index].title = payload.title;
            tasks[index].description = payload.description;
            tasks[index].size = payload.size;
            // güncel task bilgisini geri döndür
            return tasks[index];
        },
        Delete: (root, { id }) => { // id üzerinde silme işlemi operasyonu
            tasks.splice(tasks.findIndex(t => t.id === id), 1);
            return { DeletedId: id, Result: "Silme işlemi başarılı" };
        }
    }
};

/*
    ApolloServer nesnesini örnekliyoruz.
    Bunu yaparken schema, query ve resolver bilgierini de veriyoruz.
    Ardından listen metodunu kullanarak sunucuyu etkinleştiriyoruz.
    Varsayılan olarak 4000 numaralı port üzerinde yayın yapar.
*/
const houston = new ApolloServer({ typeDefs, resolvers });
houston.listen({ port: 4444 }).then(({ url }) => {
    console.log(`Houston ${url} kanalı üzerinden dinlemede`);
});
```

Bu ilk sürümü ve sonradan yazacağımız yeni versiyonu çalıştırmak için terminalden

```bash
npm run serve
```

koutunu yazmamız yeterli (Tahmin edileceği gibi package.json içerisine eklediğimiz bir run komutu var) Bunun sonucu olarak http://localhost:4444 adresine gidebilir ve otomatik olarak açılan Playground arabirimi üzerinden denemelerimizi yapabiliriz. Array kullanan bu ilk sürümün çalışma zamanına ait örnek sorguları ile ekran görüntülerini aşağıdaki bulabilirsiniz.

```bash
# Yeni bir görev eklemek
mutation {
  Insert(
    payload: {
      id: 1
      title: "Günde 50 mekik"
      description: "Kocaman göbüşün oldu. Her gün düzenli olarak mekik çekmelisin."
      size: "S"
    }
  ) {
    id
    title
    description
    size
  }
}
```

![04_38_credit_2.png](images/04_38_credit_2.png)

```bash
# Tüm görevlerin listesi
{
  AllTasks {
    title
    description
    size
    id
  }
}
```

![04_38_credit_3.png](images/04_38_credit_3.png)

```bash
# Var olan bir satırı güncelleme
mutation {
  Update(
    payload: {
      id: 1
      title: "100 Mekik"
      description: "Göbek eritme operasyonu"
      size: "M"
    }
  ) {
    id
    title
    description
    size
  }
}
```

![04_38_credit_4.png](images/04_38_credit_4.png)

```bash
# Id değerine göre görev silinmesi
mutation {
  Delete(id: 1) {
    DeletedId
    Result
  }
}
```

![04_38_credit_5.png](images/04_38_credit_5.png)

İlk sürüm önceden de belirttiğimiz üzere Apollo Server'ı basitçe işin içerisine katmak ve nasıl çalıştığını anlamak içindi. Array içeriği kalıcı bir ortamda saklanmadığından uygulama sonlandırıldığında tüm görev listesi kaybolacaktır. Kalıcı bir depolama alanı için farklı bir alternatif düşünmeliyiz. CRUD operasyonlarını başka bir servise atayabilir veya bir veri tabanı kullanabiliriz.

## İkinci Sürüm (PostgreSQL Kullanılan)

İkinci sürümde veriyi kalıcı olarak saklamak için PostgreSQL kullanıyoruz. Örneği çalışırken WestWorld'de PostgreSQL'in olmadığını fark ettim. PostgreSQL kurulumları ile ilgili olarak aşağıdaki terminal komutlarını işletmem yeterliydi.

```bash
sudo apt-get install postgresql

sudo su - postgres
psql

\l
\du
\conninfo

CREATE ROLE Scott WITH LOGIN PASSWORD 'Tiger';
ALTER ROLE Scott CREATEDB;

\q
```

İlk komut ile postgresql'i Linux ortamına kuruyoruz. Kurma işlemi sonrası ikinci ve üçüncü komutları kullanarak varsayılan kullanıcı bilgisi ile Postgresql ortmına giriyoruz. \l ile var olan veri tabanlarının listesini, \du ile kullanıcıları (rolleri ile birlikte), \conninfo ile de hangi veri tabanına hangi kullanıcı ile hangi porttan bağlandığımıza dair bilgileri elde ediyoruz. CREATE ROLE ile başlayan satırda Scott isimli yeni bir rol tanımladık. Sonrasında takip eden komutla bu role veri tabanı oluşturma yetkisi verdik. \q ile o an aktif olan oturumu kapatıyoruz. Şimdi scott rolünü kullanarak örnek veri tabanımızı ve tablolarını oluşturmaya çalışacağız.

![04_38_credit_6.png](images/04_38_credit_6.png)

```bash
psql -d postgres -U scott
CREATE DATABASE ThoughtWorld;

\list
\c thoughtworld

CREATE TABLE tasks (
  ID SERIAL PRIMARY KEY,
  title VARCHAR(50),
  description VARCHAR(250),
  size VARCHAR(2)
);

INSERT INTO tasks (title,description,size) VALUES ('Birinci Görev','Her sabah saat 06:00da kalk','L');

SELECT * FROM tasks;
```

İlk komut ile scott rolünde oturum açıyoruz. Sonrasında ThoughtWorld isimli bir veri tabanı oluşturuyoruz. \list ile var olan veri tabanlarına bakıyoruz ve \c komutuyla ThoughtWorld'e bağlanıyoruz. Ardından tasks isimli bir tablo oluşturuyor ve içerisine deneme amaçlı bir satır ekliyoruz. Son olarak basit bir Select işlemi icra etmekteyiz.

![04_38_credit_7.png](images/04_38_credit_7.png)

Artık PostgreSQL tarafı hazır. Şimdi veri tabanını Apollo suncusunda kullanmaya başlayabiliriz. Ancak öncesinde gerekli npm modülünü yüklemek lazım (Bir önceki senaryo ile kodların karışmaması adına pg-server.js isimli yeni bir dosya üzerinde çalışmaya karar verdim)

```bash
sudo npm install pg
```

pg-server.js isimli kod dosyamızın içeriği ise aşağıdaki gibi.

```javascript
const { ApolloServer, gql } = require('apollo-server');

//  postgresql kullanabilmek için gerekli modülü ekledik
const db = require('pg').Pool;
// connection string tanımı gibi düşünebiliriz.
const mngr = new db({
    user: 'scott',
    host: 'localhost',
    database: 'thoughtworld',
    password: 'Tiger',
    port: 5432
});

const typeDefs = gql`
    type Task{
        id:Int
        title:String
        description:String
        size:String
    }
    type DeleteResult{
        DeletedId:Int,
        Result:String
    }
    type Query{
        AllTasks:[Task]
        TaskById(id:Int!): Task
    }
    input TaskInput {
        title:String
        description:String
        size:String
    }
    input UpdateInput {
        id:Int!
        title:String
        description:String
        size:String
    }
    type Mutation{
        Insert(payload:TaskInput) : Task
        Update(payload:UpdateInput):Task
        Delete(id:Int!):DeleteResult
    }
`;

/*
    sorguyu göndermek için query metodundan yararlanıyoruz.
    geriye rows nesnesini döndürmekteyiz.

    query metodunun dönüşünü resolvers'tan çıkartabilmek için senkronize etmem gerekti.
    Bu nedenle async-await desenini kullandım.
*/
const resolvers = {
    Query: {
        AllTasks: async () => {
            const res = await mngr.query("SELECT * FROM tasks ORDER BY ID;")
            // console.log(res);
            if (res)
                return res.rows;
        },
        TaskById: async (root, { id }) => {
            const res = await mngr.query("SELECT * FROM tasks WHERE id=$1", [id]);
            // console.log(res);
            return res.rows[0];
        }
    },
    Mutation: {
        /*
        Yeni bir görevi eklemek için kullandığımız operasyonu da 
        async await bünyesinde değerlendirdim.
        Sorguya dikkat edilecek olursa, Insert parametrelerini 
        $1, $2 benzeri placeholder'lar ile gönderiyoruz.
        Sorgu sonucu elde edilen id değerini payload'a yükleyip geri döndürüyoruz.

        Bazı sorgularda RETURNING * kullandım. 
        Bunu yapmadığım zaman sonuç değişkenleri boş verilerle dönüyordu.
        Sebebini öğrenene ve alternatif bir yol bulana kadar bu şekilde ele alacağım.
            
        */
        Insert: async (root, { payload }) => {
            const res = await mngr.query('INSERT INTO tasks (title,description,size) VALUES ($1,$2,$3) RETURNING *',
                [payload.title, payload.description, payload.size]);
            id = res.rows[0].id;
            payload.id = id;
            return payload;
        },
        Update: async (root, { payload }) => {
            const res = await mngr.query('UPDATE tasks SET title=$1,description=$2,size=$3 WHERE ID=$4 RETURNING *', [payload.title, payload.description, payload.size, payload.id]);
            // console.log(res);
            return res.rows[0];

        },
        Delete: async (root, { id }) => {
            const res = await mngr.query('DELETE FROM tasks WHERE ID=$1', [id]);
            // console.log(res);
            return { DeletedId: id, Result: "Silme işlemi başarılı" };
        }
    }
};

const houston = new ApolloServer({ typeDefs, resolvers });
houston.listen({ port: 4445 }).then(({ url }) => {
    console.log(`Houston ${url} kanalı üzerinden dinlemede`);
});
```

Birinci senaryodaki GraphQL sorguları benzer şekilde ikinci senaryo için de denenebilir. Bu arada Visual Studio Code üzerinde PostgreSQL tarafını kolayca görüntülemek için [Chris Kolkman'nın PostgreSQL eklentisini](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres) kullandım.

![04_38_credit_8.png](images/04_38_credit_8.png)

## İstemci Tarafı

> throw new ToDoForYouException ("Bu uygulamayı size bırakıyorum. Çünkü sonraki konuya geçmek istiyorum:|");

## TODO (Eklenebilecek şeyler)

Pek tabii yapılabilecek bir kaç şey daha var. Benim aklıma gelenler şöyle;

- Dependency Injection kurgusu ile Apollo Server'ın istenen veri sağlayıcısına enjekte edilmesi için uğraşılabilinir. Örneğin tasks tablosunu SQlite ile tutmak ya da bir NoSQL sistemi üzerinden kullanmak isteyebiliriz.
- apollo-server-express modülünü kullanarak HTTPS desteğinin nasıl sağlanabileceğine bakabiliriz. Nitekim production ortamlarında HTTPS olmazsa olmazlardan.

## Ben Neler Öğrendim?

Saturday-Night-Works çalışmalarım kapsamında denediğim bu örnekte de bir sürü şey öğrendim. Sanırım aşağıdaki maddeler halinde listeleyebilirim.

- GraphQL'de tip tanımlaması (type definitions) ve çözücülerin (resolvers) ne anlama geldiğini ve neler barındırdığını
- Apollo Server paketinin kullanımını
- Insert, Update, Delete gibi operasyonların Mutation kavramı olarak ele alındığını
- CRUD operasyonlarına ait iş mekaniklerinin resolvers içindeki Query ve Mutation segmentlerinde yürütüldüğünü
- Veri kaynağı olarak farklı ortamların kullanılabileceğini (micro service, NoSQL, RDBMS, File System, REST API)
- Int? ile Int tiplerinin yerine göre doğru kullanılmaları gerektiğini (bir kaç çalışma zamanı hatası sonrası fark ettim)
- Ubuntu platformuna PostgreSQL'in kurulmasını, yeni rol oluşturulmasını, rol altında veri tabanı ve tablo açılmasını
- Apollo metodlarında pg'nin query çağrısına ait sonuçları yakalayabilmek için async-await kullanılması gerektiğini
- Visual Studio Code tarafında PostgreSQL için eklenti kullanımını

Böylece geldik bir maceramızın daha sonuna. Bu yazımızda Linux platformunda PostgreSQL kullanan stand alone çalışan bir Apollo GraphQL sunucusu yazmayı denedik. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örnek kodlarına [Saturday-Night-Works 38 Numaradan](https://github.com/buraksenyurt/saturday-night-works) erişebilirsiniz.
