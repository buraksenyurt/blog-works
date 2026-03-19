---
layout: post
title: ".Net Core Tarafından RabbitMQ'ya Mesaj Göndermek ve Java Tarafından Dinlemek"
date: 2020-11-23 21:00:00 +0300
description: "Çok sık karşılaştığımız senaryolardan birisidir; Bir uygulama kendi bünyesinde gerçekleşen bir olay sonrası başka bir uygulamayı haberdar etmek ister ya da başka bir uygulamanın yaptıklarından haberdar olmak isteyen bir uygulama vardır :) Bunun bir çok sebebi olabilir. Örneğin uygulamalar farklı teknolojilerde yazılmıştır ancak ortak iş süreçleri üzerinde koşmaktadır. Gerçek bir senaryo üzerinden hareket edersek konu daha anlaşılır olabilir. Kargo çıkışı gerçekleştiren yeni nesil bir uygulama bu çıkışlar için düzenlediği irsaliyelerin bir devlet kurumuna gönderilmesi sırasında yine aynı kurumun legacy diyebileceğimiz başka bir sisteminin süreçlerine dahil olmak durumunda olsun. Bu sürecin belli noktalarında uygulamaların birbirini haberdar etmesi gerektiğini de varsayalım. Tipik olarak gerçekleşen bir olay sonrası bu olaya ait diğer uygulamanın ihtiyacı olan bilgilerin gönderilmesi bekleniyor. Çok doğal olarak bu iletişimi senkronize etmek çok mantıklı olmaz. Nitekim anlık iş yükü çok yüksek sayılara çıkabilir ve bekleme süreleri diğer uygulamanın sürecini olumsuz etkileyebilir. Asenkron kuyruk sistemi bu dar boğazı aşmada önemli bir rol oynamaktadır."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - yaml
  - csharp
  - dotnet
  - rabbitmq
  - json
  - docker
  - java
  - serialization
  - github
---
# .Net Core Tarafından RabbitMQ'ya Mesaj Göndermek ve Java Tarafından Dinlemek
![queue.png](/assets/images/2020/queue.png)

Çok sık karşılaştığımız senaryolardan birisidir; Bir uygulama kendi bünyesinde gerçekleşen bir olay sonrası başka bir uygulamayı haberdar etmek ister ya da başka bir uygulamanın yaptıklarından haberdar olmak isteyen bir uygulama vardır:) Bunun bir çok sebebi olabilir. Örneğin uygulamalar farklı teknolojilerde yazılmıştır ancak ortak iş süreçleri üzerinde koşmaktadır. Gerçek bir senaryo üzerinden hareket edersek konu daha anlaşılır olabilir.

Kargo çıkışı gerçekleştiren yeni nesil bir uygulama bu çıkışlar için düzenlediği irsaliyelerin bir devlet kurumuna gönderilmesi sırasında yine aynı kurumun legacy diyebileceğimiz başka bir sisteminin süreçlerine dahil olmak durumunda olsun. Bu sürecin belli noktalarında uygulamaların birbirini haberdar etmesi gerektiğini de varsayalım. Tipik olarak gerçekleşen bir olay sonrası bu olaya ait diğer uygulamanın ihtiyacı olan bilgilerin gönderilmesi bekleniyor. Çok doğal olarak bu iletişimi senkronize etmek çok mantıklı olmaz. Nitekim anlık iş yükü çok yüksek sayılara çıkabilir ve bekleme süreleri diğer uygulamanın sürecini olumsuz etkileyebilir. Asenkron kuyruk sistemi bu dar boğazı aşmada önemli bir rol oynamaktadır.

Öyleyse çok basit bir kurgu ile bunu kendi sistemlerimizde uygulamaya çalışalım. Örnek çalışmadaki amacımız RabbitMQ'yu Heimdall üstünde olabilecek en basit haliyle çalıştırmak, bir.Net Core Console uygulamasından belli bir konu başlığı (topic) için bu kuyruğa mesaj bırakmak ve oldukça yabancısı olduğum Spring tarafındaki bir Java uygulamasından da gönderilen mesajları yakalamak. Öyleyse hiç vakit kaybetmeden işe başlayalım. Örneği deneyimlediğim Heimdall (Ubuntu 20.04) üzerinde koşan bir RabbitMQ hizmeti mevcut değil. Aslında kurmayı da istemiyorum. Docker veya Docker-Compose kullanmak çok daha mantıklı.

Aşağıdaki terminal komutları ile başlayalım.

```bash
touch docker-compose.yml
# Çalıştırmak için
docker-compose up
```

docker-compose.yml içeriğini aşağıdaki gibi oluşturabiliriz. Dikkat edileceği üzere RabbitMQ imajını kullanarak gerekli port ayarlamaları ile birlikte sistemi ayağa kaldırıyoruz.

```yml
rabbitmq:
    image: rabbitmq:management
    ports:
      - "5672:5672"
      - "15672:15672"
```

Mesaj gönderecek.Net Core uygulamasını oluşturmak içinse aşağıdaki adımları takip edebiliriz.

```bash
# SENDER
# RabbitMQ'ya mesaj gönderecek uygulamamız bir .Net Core uygulaması olacak
dotnet new console --name CargoBase
# Gerekli Nuget paketleri de aşağıdaki gibi ekleyebiliriz
# Birisi RabbitMQ ile konuşmak için
dotnet add package RabbitMQ.Client
# Diğer Console uygulamasında nesneyi JSON serileştirmekte yardımcı olsun diye
dotnet add package Newtonsoft.Json
```

CargoBase isimli uygulamaya ait program kodunuysa aşağıdaki gibi yazabiliriz.

```csharp
using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace CargoBase
{
    class Program
    {
        static void Main(string[] args)
        {

            Random _random = new Random();
            // Factory nesnesi üstünden RabbitMQ'ya bir bağlantı açacağız
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                // sonrasında kanal tanımlama ve mesaj gönderme işi için gerekli nesneleri üreteceğiz
                using (var channel = connection.CreateModel())
                {
                    string queueName = "package-state-action";
                    // Bir kuyruk tanımladık. Kargonun durum değişikliği ile alakalı bir kuyruk gibi düşünelim
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // Kuyruğu JSON olarak serileştirilmiş bir nesne koyalım. Kobay nesnemiz Package türünden bir örnek.
                    var package = JsonConvert.SerializeObject(
                            new Package
                            {
                                SerialNo = _random.Next(1, 1000),
                                State = "Ready",
                                Weight = _random.NextDouble()*100,
                                Time = DateTime.Now.ToString()
                            });
                    // nesne içeriğini kanala yazmak için Byte[] dizisine çeviriyoruz
                    var body = Encoding.UTF8.GetBytes(package);
                    Console.WriteLine($"{package} içeriği gönderilecek");

                    // routingKey bilgisi ile de yukarıda tanımlanan kanala mesajımızı bırakalım
                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                }

                Console.WriteLine("Kargo için durum bilgisi yayınlandı. Çıkmak için bir tuşa basınız");
                Console.ReadLine();
            }
        }
    }

    public class Package
    {
        public int SerialNo { get; set; }
        public string State { get; set; }
        public double Weight { get; set; }
        public string Time { get; set; }
    }
}
```

Konsolumuz bazı rastgele değerlerden oluşan bir paket bilgisini kuyruğa göndermekte. Alıcı uygulamayı Java tarafında geliştireceğiz ama işimizi kolaylaştırmak adına Spring Boot'tan yararlanacağız. Bu nedenle [Spring Initializer](https://start.spring.io/) adresine gidip gerekli proje bilgilerini doldurup oluşan uygulamayı sisteme indirip kullanabiliriz. Ben ilgili bilgileri aşağıdaki ekran görüntüsünde olduğu gibi doldurdum. Tabii burada kritik nokta Spring for RabbitMQ kütüphanesinin de bağımlı bileşen olarak belirtilmesi.

![skynet_38_Screenshot_01.png](/assets/images/2020/skynet_38_Screenshot_01.png)

Java projesi oluştuktan sonra RabbitMQ tarafını dinleyecek ve gelen JSON mesajını nesne olarak karşılayacak sınıfları da yazmamız gerekiyor (JSON serileştirme için com.fasterxml.jackson.core bağımlılığının da eklenmesi gerekir. Ben ilk etapta unutmuşum, siz ihmal etmeyin)

```bash
# RabbitMQ mesajlarını dinleyecek java servisini
# src/main/java/com/azon/cargotracer altında oluşturabiliriz
touch EventListener.java PackageInfo.java
```

EventListener.java içeriğini aşağıdaki gibi yazabiliriz.

```csharp
package com.azon.cargotracer;

import com.fasterxml.jackson.databind.ObjectMapper;

import org.springframework.amqp.core.Message;
import org.springframework.amqp.core.MessageListener;
import org.springframework.stereotype.Service;

/*
    MessageListener'dan türeyen bu sınıfın ezdiğimiz(override)
    onMessage metodu üzerinden, RabbitMQ tarafında ilgili kuyruğa atılmış mesajın gövdesini yakalayabiliriz.
*/
@Service
public class EventListener implements MessageListener {

    public void onMessage(Message message) {
        // Message tipinin getBody fonksiyonu ile kuyruk mesajının içeriğini aldık
        String content = new String(message.getBody());
        System.out.println("\n" + content + "\n");

        try {
            /*
                İçeriği JSON formatında göndermiştik.
                Jackson isimli paketten yararlanarak bu içeriği Java tarafındaki PackageInfo nesnemize dönüştürebiliriz.
                Gelen JSON içeriğini Java tarafında nesne olarak ele alabilmek için...
            */
            ObjectMapper objectMapper = new ObjectMapper();
            PackageInfo packageInfo = objectMapper.readValue(content, PackageInfo.class);
            System.out.println(
                    packageInfo.SerialNo + "," + packageInfo.State + "," + packageInfo.Weight + "," + packageInfo.Time);
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }
}
```

PackageInfo.java;

```csharp
package com.azon.cargotracer;

/*
    .Net Core tarafında RabbitMQ kuyruğuna attığımız Package sınıfı JSON formatta serileşerek yollanıyor.
    Onun Java tarafındaki izdüşümü kabul edeceğimiz sınıfı aşağıdaki gibi tanımlayabiliriz.
*/
public class PackageInfo {

    public int SerialNo;
    public String State;
    public double Weight;
    public String Time;

    public int getSerialNo() {
        return SerialNo;
    }

    public void setSerialNo(int value) {
        SerialNo = value;
    }

    public String getState() {
        return State;
    }

    public void setState(String value) {
        State = value;
    }

    public double getWeight() {
        return SerialNo;
    }

    public void setWeight(double value) {
        Weight = value;
    }

    public String getTime() {
        return Time;
    }

    public void setTime(String value) {
        Time = value;
    }
}
```

## Çalışma Zamanı

Kurguyu işletmek için üç uygulama çalıştırmamız gerekiyor. Öncelikle docker-compose ile RabbitMQ tarafını ayağa kaldırmalıyız. Ardından mesaj gönderecek olan dotnet core uygulamasını başlatabiliriz. Java uygulaması çalıştırıldıktan sonra ilgili RabbitMQ kuyruğunu dinleme konumunda kalacaktır. Onu dotnet core uygulamasından önce de çalıştırabiliriz. Sonuç itibariyle.Net uygulamasından mesaj basıldıkça Java arabirimine çıkması gerekmektedir.

```bash
# Rabbit tarafı için
docker-compose up

# Console uygulaması için (Mesaj gönderen taraf)
dotnet run

# Maven ile Java tarafını başlatmak için
./mvnw spring-boot:run
```

İşte çalışma zamanından bir görüntü.

![skynet_38_Screenshot_03.png](/assets/images/2020/skynet_38_Screenshot_03.png)

Dikkat edileceği üzere Console uygulamasından gönderdiğimiz JSON mesaj içeriği, Java uygulamasına ait terminal ekranına da düşmüştür. Bu arada uygulama kodlarına [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2038%20-%20Spring%20RabbitMQ%20and%20DotNetCore) üzerinden erişebilirsiniz. Kodlara eriştiğinizde şu soruya cevap aramanızı öneririm; Varsayılan halde Java uygulaması localhost sunucusuna ve standart RabbitMQ portuna gideceğini nereden biliyor? Bu sorulara ek olarak kurguyu biraz daha öteye taşıyabilirsiniz. Örneğin Java uygulamasını birden fazla kuyruğu dinleyecek şekilde organize etmeyi deneyebilir ve hatta kullandığınız kuyruğa başka platformda yazılmış programlardan mesaj gönderip kimden geldiğini anlamaya çalışabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.