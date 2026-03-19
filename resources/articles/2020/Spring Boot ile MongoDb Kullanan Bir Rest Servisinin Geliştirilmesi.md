---
title: "Spring Boot ile MongoDb Kullanan Bir Rest Servisinin Geliştirilmesi"
pubDate: 2020-10-11 19:01:00
categories:
  - Spring Boot
tags:
  - Spring Boot
  - Bash
  - C#
  - Java
  - PostgreSQL
  - MongoDB
  - XML
  - REST
  - JSON
  - HTTP
  - Docker
  - Dependency Injection
  - Visual Studio
  - GitHub
  - Dependency Management
---

# Spring Boot ile MongoDb Kullanan Bir Rest Servisinin Geliştirilmesi
![springboot.png](images/springboot.png)

Spring Boot, Java kod tabanı üzerine oturmuş ve özellikle kurumsal çapta uygulamaların geliştirilmesinde önemli bir yere sahip olan Spring çatısınının kullanımını oldukça kolaylaştıran,basitleştiren zevkli hale getiren bir başka çatıdır (Framework). Dahili Dependency Injection mekanizması ve zengin paket desteği sayesinde otonom araçlardan akıllı televizyonlara, elektronik ticaretten bulut uygulamalara kadar birçok alanda Spring'in kabiliyetlerini oldukça etkin kullanabilmemize olanak sağlamaktadır. Birazdan sizin de göreceğiniz üzere az eforla oldukça etkili bir servis ortaya çıkaracağız.

Geliştireceğimiz örnekteki amacımız [resmi dokümantasyondan](https://spring.io/guides/gs/spring-boot/) da yararlanarak MongoDb veritabanını kullanan basit bir REST servisi yazmak. Spring hayatı o kadar kolaylaştırıyor ki, onunla geliştirilen bir API servisi pek çok standardı da otomatik olarak sağlıyor. Haydi gelin nasıl olduğuna bir bakalım.

Ben bu çalışmayı Heimdall (Ubuntu-20.04) üstünde ve bir numaralı geliştirme arabirimi olarak kabul ettiğim Visual Studio Code ile geliştirmekteyim. Çalışmaya başladığınızda sizin de sisteminizde Java ve build mekanizması için kullanılması gereken Gradle ya da Maven olmayabilir. Bu nedenle aşağıdaki terminal komutlarına benzer bir kurulum sürecinden geçmeniz gerekebilir. Ek olarak bu örnek özelinde sisteme MongoDB kurmak yerine her zaman olduğu gibi Docker imajından yararlanabiliriz.

```bash
# Ubuntu 20.04 tarafındaki kurulum için
sudo apt install openjdk-14-jdk

# Sonrasın bir versiyon kontrolü
java --version

# Apache Maven kurulumu içinse
sudo apt install maven

# Yine bir çalışıyor mu kontrolü tabii
mvn -version

# MongoDb için daha önceki örneklerde olduğu gibi docker imajı kullanmayı tercih ettim.
sudo docker run --name mongodb -p 27017:27017 -d mongo:latest
```

Spring Boot tarafında belki de en önemli yardımcımız uygulama iskeletini bağımlılıkları ile birlikte kolayca oluşturmamızı sağlayan Spring Initializer isimli çevrimiçi araç ([Şu adresten](https://start.spring.io/) ulaşabilirsiniz) Bu yazıdaki örneğimiz için aşağıdaki ekran görüntüsünde yer alan konfigurasyonu kullanacağız. Uygulamaya REST özellikleri katmak için Rest Repositories ve Mongo tarafı ile kolayca konuşabilmek için Spring Data MongoDB bağımlılıklarını eklediğimize dikkat edelim.

![skynet_27_Screenshot_01.png](images/skynet_27_Screenshot_01.png)

Arayüzde gerekli bilgileri doldurduktan sonra Generate butonuna basmamız yeterli. Sisteme sıkıştırılmış bir uygulama paketi inecektir. İndirilen hazır kod deposu üzerinde gerekli geliştirmelerimizi yapabiliriz. Tabii başlamadan önce neler gelmiş diye bakmakta yarar var. Sözgelimi yukarıdaki görselde belirlediğimiz ayarlar pom.xml dosyası içerisine yazılacaktır vb...

MongoDB tarafında örnek olarak oyuncu bilgilerini tutacağımız bir doküman kullanabiliriz. Bu dokümanın Java tarafındaki izdüşümünü bir sınıf (POJO-Plain Old Java Object) ile ifade edebiliriz. Söz konusu sınıfı src/main/java/com/skynet/gamesworldapi altında Player ismiyle aşağıdaki gibi oluşturarak çalışmamıza devam edelim.

```csharp
package com.skynet.gamesworldapi; // Hangi pakete dahil

import org.springframework.data.annotation.Id;

/*
    MongoDb tarafındaki veriyi işaret eden eş nesne.
    Bir POJO (Plain Old Java Object)
    Lakin, C# Auto Property'lerin gözünü seveyim :D
*/
public class Player {

    @Id
    private String id; // import edilen paket üstünden kullandığımız Field. MongoDB ObjectId için
                       // kullanıyoruz. @Id niteliği ile bunu ifade etmiş olduk

    private String _nickName;
    private Integer _level;
    private Boolean _isActive;

    public String getNickName() {
        return _nickName;
    }

    public void setNickName(String nickName) {
        _nickName = nickName;
    }

    public Integer getLevel() {
        return _level;
    }

    public void setLevel(Integer level) {
        _level = level;
    }

    public Boolean getIsActive() {
        return _isActive;
    }

    public void setIsActive(Boolean isActive) {
        _isActive = isActive;
    }
}
```

MongoDb ile olan servis iletişimi Player nesnesine göre otomatik olarak kurgulanacaktır. Lakin ek operasyon desteği de sunmak isteyebiliriz. Örneğin aktif veya pasif oyuncu listesini verecek bir fonksiyonellik gibi. Bunun için MongoDb eklentisi ile gelen MongoRepository türünü genişletmemiz gerekiyor. Genişletilen tip sisteme enjekte edileceğinden interface olarak tasarlanmalıdır. @RepositoryRestResource niteliği ise REST için gerekli path bildirimini barındırmaktadır. Yani yönlenilen REST Endpoint'in hangi sözleşme tarafından karşılanacağı çalışma zamanına annotation üzerinden söylenir.

```java
package com.skynet.gamesworldapi;

import java.util.List; // Metodumuz bir liste döndüreceği için eklenen paket

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.repository.query.Param;
import org.springframework.data.rest.core.annotation.RepositoryRestResource;

/*
    MongoRepository türünü genişleten bir interface söz konusu.
    İçinde etkin olup olmama durumuna göre oyuncu listesi döndüren bir metodumuz da var.
    path ile bu repository için API EndPoint'ini tanımlamış olduk
*/

@RepositoryRestResource(collectionResourceRel = "player", path = "player")
public interface PlayerRepository extends MongoRepository<Player, String> {
    /*
        Geriye Player türünden bir liste döndürecek.
        Amaç isActive değerine göre etkin olan veya olmayan oyuncu listesini çekmek
        _isActive.
        Bu arada metot adının getBy_isActive olması tesadüf değil. Player sınıfındakinda isActive field'ını bu şekilde isimlendirdiğimiz için.
        Aksi durumda build sırasında hata alırız. Kod derlenmez. Test çıktısı da fail olur.
    */
    List<Player> getBy_isActive(@Param("active") Boolean isActive);
}
```

Aslında tüm hazırlıklar bu kadar:) Uygulamayı Maven ile doğrudan çalıştırabiliriz. Ya da bir paket derleyip onu da yürütebiliriz. Her iki kullanımı da aşağıdaki terminal komutlarında görebilirsiniz (mvnw dosyası bu örnek özelinde games-world-api klasörü altındadır)

```bash
./mvnw spring-boot:run

# ya da paketi derleyip üretilen JAR dosyası ile de çalıştırılabilir
./mvnw clean package
java -jar target/games-world-api-0.0.1-SNAPSHOT.jar
```

Buna göre aşağıdakine benzer bir çalışma zamanı elde etmemiz gerekiyor.

![skynet_27_Screenshot_02.png](images/skynet_27_Screenshot_02.png)

Gelelim testlerimize. Uygulamamız malumunuz bir REST servisi. Bu nedenle Curl veya Postman gibi araçlarla test yapabiliriz. Dikkat edilmesi gereken nokta özellikle kendi eklediğimiz dışında tüm CRUD (Create Read Update Delete) fonksiyonlarının karşılığı olan Post, Get, Put, Delete operasyonlarının hazır olarak sunulmasıdır. Lütfen bunlar için herhangi bir kod yazmadığımıza dikkat edin. Sihir!;) Öyleyse birkaç talep ile devam edelim.

Yeni bir oyuncuyu aşağıdaki basit talep ile ekleyebiliriz.

```text
http://localhost:8080/player
POST
JSON Body
{
	"nickName": "bamble bee",
	"level": 55,
	"isActive": true
}
```

![skynet_27_Screenshot_03.png](images/skynet_27_Screenshot_03.png)

Birkaç oyuncu daha ekleyip sonrasında tüm oyuncu listesini almak için aşağıdaki talebi kullanmak yeterli.

```text
http://localhost:8080/player
GET
```

Tabii listelenen verinin büyük olma olasılığına karşın Spring Boot sayfalama özelliklerini de hesaba katmaktadır. Aşağıdaki görüntüde yer alan page kısmına dikkat edelim.

![skynet_27_Screenshot_04.png](images/skynet_27_Screenshot_04.png)

Belli bir dokümanı (player verisini) çekmek istersek MongoDB'nin kendine has ID bilgisini kullanmamız yeterli. Mesela;

```text
http://localhost:8080/player/5f3d626214851c46fd10544a
GET
```

![skynet_27_Screenshot_05.png](images/skynet_27_Screenshot_05.png)

Bir içeriği silmek için,

```text
http://localhost:8080/player/5f3d625114851c46fd105449
DELETE
```

![skynet_27_Screenshot_07.png](images/skynet_27_Screenshot_07.png)

veya veri güncellemek içinse (Komple set) aşağıdaki örnek talebi kullanabiliriz.

```text
http://localhost:8080/player/5f3d626214851c46fd10544a
PUT
JSON Body

{
	"level": 46,
	"nickName": "Sala Mura Jack",
	"isActive": false
}
```

![skynet_27_Screenshot_08.png](images/skynet_27_Screenshot_08.png)

Hatırlayacağınız üzere MongoDb repository'sini kendi sözleşmemiz ile genişletmiştik. Pasif ve aktif oyuncuların listesini çekmek için bu fonksiyona doğru aşağıdaki talepleri yollayabiliriz. Pasif olan oyuncular için,

```text
http://localhost:8080/player/search/getBy_isActive?active=false
GET
```

ve aktif oyunclar için

```text
http://localhost:8080/player/search/getBy_isActive?active=true
GET
```

İşte sonuçlar;

![skynet_27_Screenshot_06.png](images/skynet_27_Screenshot_06.png)

Görüldüğü üzere Spring Boot kullanarak MongoDB ile konuşan bir REST servisini temel CRUD ve bizim ekleyeceğimiz ilave fonksiyonlar ile ayağa kaldırmak oldukça basit, zahmetsiz ve kolay;) Veri odaklı servisleri kolaylıkla üretim ortamlarına alabiliriz diye düşünüyorum. Spring Boot konusunda deneyimleriniz varsa bu konu özelinde yorumlarınız olursa lütfen yazının altına bırakmaya çekinmeyin. Bu arada örneği geliştirmek elbette elinizde. Söz gelimi MongoDb yerine PostgreSQL kullanmayı deneyebilirsiniz. Hatta oyuncuların katıldıkları maçlara ait skor, başarı, madalya, seviyle vb bilgilerin tutulduğu yeni bir POJO hazırlayıp Player ile ilişkilendirerek servis üzerinden sunmayı düşünebilirsiniz;)

Böylece geldik bir Skynet derlememizin daha sonuna. Örneğe ait kodlara [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2027%20-%20REST%20with%20Spring%20and%20MongoDb) erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
