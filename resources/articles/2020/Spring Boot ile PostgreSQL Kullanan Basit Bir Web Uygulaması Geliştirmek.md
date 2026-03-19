---
title: "Spring Boot ile PostgreSQL Kullanan Basit Bir Web Uygulaması Geliştirmek"
pubDate: 2020-10-19 07:20:00
categories:
  - Spring Boot
tags:
  - Spring Boot
  - Bash
  - C#
  - Java
  - PostgreSQL
  - MongoDB
  - REST
  - HTTP
  - Docker
  - Generics
  - Dependency Injection
  - Visual Studio
  - GitHub
  - Dependency Management
---

# Spring Boot ile PostgreSQL Kullanan Basit Bir Web Uygulaması Geliştirmek
![category.png](images/category.png)

Spring Boot maceralarımız hız kesmeden devam ediyor. Bu sefer PostgreSQL veritabanını kullanan bir Web uygulamasını resmi dokümandan da yararlanarak geliştirmeye çalışacağız. Örneğimizde veritabanı olarak PostgreSQL kullanabiliriz. Her zaman olduğu gibi sisteme kurmamız şart değil. Pekala Docker imajından yararlanabiliriz. Kurgumuz basit bir MVC düzeneği olacak. Statik bir web sayfası dışında listeleme ve yeni kategori ekleme adımlarında şablonlardan (templates) faydalanacağız. Kategorileri ifade eden bir POJO sınıfımız olacak. PostgreSQL bağımlılığı kapsamında temel CRUD operasyonlarının tamamı Spring Boot'e ekleyeceğimiz bağımlılık sayesinde zaten hazır gelecek. Bunu kategori türüne uygulamak içinse generic bir Repository arayüzünden türetme yoluna gideceğiz.

Model ile View arasında köprü vazifesi gören Controller tipi, gerekli CRUD operasyonlarına erişmek için bir sözleşme arayüzünü kullanacak. Tahmin edeceğiniz üzere asıl operasyonları kullanması için Controller tipine ihtiyacı olan nesneyi, Dependency Injection mekanizması yardımıyla aktaracağız. Kod kısmını sırayla takip ettiğinizde konuyu daha iyi anlayacağınızdan eminim. Ben örneğimizi Heimdall (Ubuntu-20.04) üzerinde ve Visual Studio Code arabirimini kullanarak geliştirmekteyim. Ancak temel olarak tüm platformlarda benzer adımlarla ilerleyeceğinizi ifade edebilirim. Öyleyse gelin PostgreSQL Container'ını hazırlayarak çalışmamıza başlayalım.

```bash
# Container'ı Tokyo ismiyle ayağa kaldıralım
sudo docker run --name Tokyo -e POSTGRES_PASSWORD=P@ssw0rd -p 5432:5432 -d postgres
# Üzerinde bash açıp
sudo docker exec -it Tokyo bash
# PostgreSQL veritabanımızı oluşturalım
psql -U postgres
Create Database qworld;
```

Veritabanı tarafı hazır. Sırada uygulamanın inşası var. İlk iş olarak [Spring Initializr](https://start.spring.io/) adresine gidip POM içeriğini ve uygulamayı hazırlamak lazım. Veritabanı kullanımı için PostgreSQL Driver, temel web uygulaması kabiliyetleri için Spring Web, MVC şablonlarını kullanabilmek için Thymeleaf (ki bunu bir türlü telaffuz edemiyorum), Object Relational Map aracı Hibernate içinse Spring Data JPA kütüphanelerini yüklüyoruz.

![skynet_31_Screenshot_01.png](images/skynet_31_Screenshot_01.png)

Arabirimin ürettiği uygulamayı sisteme indirdikten sonra aşağıdaki adımları takip ederek senaryomuz için gerekli kod dosyalarını oluşturabiliriz.

```bash
cd quote-world-web
# Model klasörü ve sınıfı
mkdir src/main/java/com/learning/quoteworldweb/model
touch src/main/java/com/learning/quoteworldweb/model/Category.java

# Repository klasörü ve sınıfı
mkdir src/main/java/com/learning/quoteworldweb/repository
touch src/main/java/com/learning/quoteworldweb/repository/CategoryRepository.java

# Servis sözleşmesi ve sınıfı
mkdir src/main/java/com/learning/quoteworldweb/service
touch src/main/java/com/learning/quoteworldweb/service/ICategoryService.java src/main/java/com/learning/quoteworldweb/service/CategoryService.java

# Controller klasörü ve sınıfı
mkdir src/main/java/com/learning/quoteworldweb/controller
touch src/main/java/com/learning/quoteworldweb/controller/CategoryController.java

# statik indeks sayfası
touch src/main/resources/static/index.html

# Kategorileri listelemek ve yeni bir tane eklemekte kullanılmak üzere iki template sayfası
touch src/main/resources/templates/allCategories.html src/main/resources/templates/newCategory.html

# Veritabanı tablo şeması ve örnek veri girişleri için ilgili sql dosyaları
# application.properties dosyasındaki ayarlara göre uygulama başlarken schema dosyasına bakıp eğer yoksa tabloyu oluşturmalı
# ve örnek verileri eklemeli
touch src/main/resources/schema.sql src/main/resources/data.sql
```

Gelelim kodlarımıza. İlk olarak bir kategoriyi temsil eden POJO (Plain Old Java Object) sınıfı ile işe başlayalım (Kodları sadece Copy-Paste yapmayın. Yorum satırlarını da mutlaka okuyun)

```csharp
package com.learning.quoteworldweb.model;

/*
    Model sınıfımız. Yani Entity nesnemiz.
*/
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name="categories") // Veritabanındaki categories tablosunu işaret ettiğini belirtiyoruz
public class Category{

    // Tablodaki otomatik artan Identity alanımız

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String title;
    private Integer quotecount;

    // Varsayılan yapıcı metodumuz
    public Category()
    {
    }

    // Parametrik yapıcı metodumuz
    public Category(Long id,String title,Integer quotecount)
    {
        this.id=id;
        this.title=title;
        this.quotecount=quotecount;
    }

    public Long getId()
    {
        return this.id;
    }

    public String getTitle()
    {
        return this.title;
    }

    public void setTitle(String value)
    {
        this.title=value;
    }

    public Integer getQuotecount()
    {
        return this.quotecount;
    }

    public void setQuotecount(Integer value)
    {
        this.quotecount=value;
    }
}
```

Varsayılan CRUD (Create Read Update Delete) operasyonlarını barındıran Repository sözleşmesini Category tipi için uygulayacağımızı sisteme bir şekilde söylememiz lazım. Bu nedenle generic CrudRepository'den türetilen bir Interface tipi söz konusu. Spring ile REST servisi geliştirdiğimiz örnekte de benzer bir yaklaşım olduğunu hatırlarsınız. Bu sözleşme içerisinde başka bir operasyon bildirimi henüz yok ancak dilerseniz genişletebilir ek fonksiyonellikleri de Repository'ye dahil edebilirsiniz.

```java
package com.learning.quoteworldweb.repository;

import com.learning.quoteworldweb.model.Category;

import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

/*
    Standart CRUD operasyonlarını devraldığımız repository arayüzü.
*/
@Repository
public interface CategoryRepository extends CrudRepository<Category, Long> {
}
```

Sırada Controller tarafından kullanılacak olan servis sözleşmesi var. Bu sözleşme Controller tipine DI üzerinden dahil edileceğinden bir Interface ve uyarlamasına ihtiyacımız var. ICategoryService ve CategoryService tiplerini aşağıdaki gibi geliştirebiliriz.

ICategoryService;

```java
package com.learning.quoteworldweb.service;

import java.util.List;
import com.learning.quoteworldweb.model.Category;

public interface ICategoryService {
    List<Category> getAll();

    Category getSingle(Long id);

    Long add(Category category);
}
```

CategoryService;

```csharp
package com.learning.quoteworldweb.service;

import java.util.List;
import com.learning.quoteworldweb.model.Category;
import com.learning.quoteworldweb.repository.CategoryRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

/*
    Tüm kategorileri ve bir id değerine göre tek kategoriyi döndüren operasyonları içeren servis sınıfımız.
    ICategoryService arayüzünü implemente ettiği için oradaki metodları ezmek zorundayız.
    findAll ve findById gibi fonksiyonlar CategoryRepository isimli repository sınıfı üzerinden kullanılmaktadır.
    add Metodunu ise yeni bir kategoriyi eklemek için kullanmaktayız.
*/

@Service
public class CategoryService implements ICategoryService {

    @Autowired
    private CategoryRepository repository; // Repository sınıfı enjekte ediliyor

    @Override
    public List<Category> getAll() {
        return (List<Category>) repository.findAll();
    }

    @Override
    public Category getSingle(Long id) {
        return repository.findById(id).get();
    }

    @Override
    public Long add(Category category){
        Long id=repository.save(category).getId();
        return id;
    }
}
```

Artık Controller için gerekli enstrümanlarımız hazır. Model ile View tarafını bağlayan CategoryController sınıfını aşağıdaki gibi yazarak çalışmamıza devam edelim.

```csharp
package com.learning.quoteworldweb.controller;

import java.util.List;

import com.learning.quoteworldweb.model.Category;
import com.learning.quoteworldweb.service.ICategoryService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;

@Controller
public class CategoryController {
    @Autowired
    private ICategoryService categoryService; // Servis örneği enjekte ediliyor

    @GetMapping("/allCategories") // Path tanımı
    public String allCategories(Model model) {
        var result = (List<Category>) categoryService.getAll(); // Enjekte edilen servis üstünden tüm kategori listesi
                                                                // çekildi
        model.addAttribute("categoryList", result); // İlişkili model nesnesine attibute olarak ilgili liste eklendi
        return "allCategories"; // Model nesnesi, thymeleaf sayesinde allCategories.html dosyasına bağlanacak

        /*
         * Model üstünden categoryList değişkeni ile geriye döndürdüğümüz bir liste söz
         * konusu. allCategories.html dosyasında model'den gelen Category nesnelerini
         * HTML'e nasıl bağladığımıza dikkat edin.
         * 
         * Ayrıca yeni kategori eklemek için farklı bir view kullanılıyor. newCategory
         * path'ine gelen talepler newCategory.html şablonunu döndürmekte.
         * 
         * newCategory.html şablonundaki form HTTP Post ile yollandığındaysa PostMapping
         * niteliği ile işaretlenmiş olan addCategory metodu çalışıyor. Form
         * elementinden gelen Category nesne örneği,CategoryService aracılığıyla
         * Postgresql veritabanına kayıt ediliyor. Sonrasında ana sayfaya yönlendirme
         * yapıyoruz.
         */
    }

    @GetMapping("newCategory")
    public String newCategory(Model model) {
        model.addAttribute("category", new Category());
        return "newCategory";
    }

    @PostMapping("/addCategory")
    public String addCategory(Model model, @ModelAttribute("category") Category c) {
        // TODO Exception durumunu kontrol edip bir HTTP Status mesajı vermeyi
        // deneyebiliriz
        categoryService.add(c);
        return "redirect:/allCategories/";
    }
}
```

Bitti mi? Bitmedi:) Önyüzden ne haber? Oldukça ilkel HTML şabonlarımızı da sırasıyla yazalım. Web uygulamamızın giriş sayfası index.html. Sadece diğer sayfalara yönlendirme yapan linkler barındırıyor.

```text
<html>
    <head>
        <title>Alıntı Dünyası</title>
    </head>
    <body>
        <div>
            <h2>Alıntı Dünyasına Hoşgeldiniz</h2>
            <a href="allCategories">Tüm Kategoriler için tıklayın</a><br/>
            <a href="newCategory">Kategori eklemek için tıklayın</a>
        </div>
    </body>
</html>
```

Yeni bir kategori eklemek için kullanacağımız newCategory sayfasını da aşağıdaki gibi geliştirebiliriz. Burada tahmin edileceği üzere bir POST işlemi söz konusu. Hangi Action'a bağlanacağımız th:action ile belirtilirken kullanlacak model nesnesi de th:object bildirimleri ile belirtmekteyiz. Category alanları ile HTML elementlerini bağlarken ise th:field bildirimi devreye giriyor.

```text
<html>
    <head>
        <title>Yeni Kategori</title>
    </head>
    <body>
        <div>
            <h2>Yeni Kategori</h2>
            <form th:action="@{addCategory}" th:object="${category}" method="POST">
                <table>
                    <tr>
                        <td>Başlık</td>
                        <td><input type="text" th:field="*{title}" /></td>
                    </tr>
                    <tr>
                        <td>Alıntı Sayısı</td>
                        <td><input type="text" th:field="*{quotecount}" /></td>
                    </tr>
                    <tr>
                        <td><input type="submit" value="Kaydet"/></td>
                        <td></td>
                    </tr>
                </table>
            </form>
        </div>
    </body>
</html>
```

ve son olarak tüm kategorileri gösteren allCategories sayfamız. Sayfanın bağlandığı modeldeki categoryList koleksiyonunun elemanlarını dolaşırken th:each bildirimi devreye giriyor. Her bir kategori nesnesinin alanlarına ise bu nesne üzerinde nokta notasyonu ile (c.title gibi) erişiyoruz.

```text
<html>
    <head>
        <title>Kategoriler</title>
    </head>

    <body>
        <div>
            <h2>Kategoriler</h2>
            <table>
                <tr>
                    <th>Id</th>
                    <th>Başlık</th>
                    <th>Alıntı Sayısı</th>
                </tr>
                <tr th:each="c : ${categoryList}">
                    <td th:text="${c.id}">Id</td>
                    <td th:text="${c.title}">Başlık</td>
                    <td th:text="${c.quotecount}">Alıntı Sayısı</td>
                </tr>
            </table>
            <p>
                <a href="/">Ana sayfa</a>
            </p>
     </div>
    </body>
</html>
```

Bu arada PostgreSQL tarafındaki nesne oluşumları için hazırlanan script'leri de atlamayalım. Schema.sql içerisinde categories tablosunu oluşturan script yer alıyor.

```text
CREATE TABLE categories(id serial PRIMARY KEY, title varchar(50),quotecount integer);
```

Örnek birkaç veri içinse Data.sql içeriğini kullanabiliriz.

```text
INSERT INTO categories(title,quoteCount) VALUES ('Türk Edebiyatından',150);
INSERT INTO categories(title,quoteCount) VALUES ('Futuristlerde',58);
INSERT INTO categories(title,quoteCount) VALUES ('İlham Veren',18);
```

Peki tabii arabirim çok ilkel. Bootstrap veya muadili yapıları kullanarak görsel yönü çok daha zengin bir tasarım hazırlanabilir. Uygulamayı maven üzerinden aşağıdaki terminal komutu hemen çalıştırabiliriz. Sonrasında localhost:8080 portuna gitmemiz yeterli olacaktır.

```bash
./mvnw spring-boot:run
```

İşte Index sayfamız,

![skynet_31_Screenshot_02.png](images/skynet_31_Screenshot_02.png)

ve kategorilere gittiğimizde göreceğimiz sayfa.

![skynet_31_Screenshot_03.png](images/skynet_31_Screenshot_03.png)

Yeni kategori ekleme sayfası ise aşağıdaki gibi görünecektir.

![skynet_31_Screenshot_04.png](images/skynet_31_Screenshot_04.png)

Son olarak yeni eklenen kategorinin listeye geldiğini gördüğümüzden emin olalım.

![skynet_31_Screenshot_05.png](images/skynet_31_Screenshot_05.png)

Yanlış bir kategori mi eklediniz? Var olanı silmek mi istiyorsunuz? Vay halinize:D Benim üşenip de yazmadığım bu action'lar size bir görev olsun. Kodları incelerken şu sorulara cevap bulmaya çalışırsanız konuyu daha da pekiştirebilirsiniz. En azından benim aklıma gelenler bunlar.

- Template tarafı model nesnesinin ilgili alanlarıyla nasıl bağlantı kuruyor?
- Sizce örnek tipik bir Repository Pattern uyarlaması mı?
- CategoryController sınıfındaki newCategory metodunda model nesnesinin attribute'larına yeni bir Category nesnesi eklememizin sebebi nedir? Eklemezsek ne olur?

Bu sorulara ek olarak uygulamaya yeni bir Entity nesnesini (örneğin kategorilere bağlı kitap alıntlarını tutan sınıfı) dahil edebilir, PostgreSQL yerine MongoDB kullanmayı deneyebilirsiniz. Hoş bir tasarıma da kavuşturduktan sonra aslında çok temel ihtiyaçları sağlayan veri odaklı bir MVC uygulaması yazmış oluyorsunuz. Bence güzel;) Böylece geldik bir SkyNet derlememizin daha sonuna. Kodların tamamına [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2031%20-%20Web%20App%20with%20Spring%20and%20PostgreSQL) ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
