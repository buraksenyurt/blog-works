---
layout: post
title: "Python ve Flask ile REST Tabanlı Servis Geliştirmek"
date: 2016-09-22 22:00:00 +0300
categories:
  - python
tags:
  - python
  - bash
  - dotnet
  - rest
  - json
  - http
  - java
  - ruby
  - microservices
---
Bir süredir [Python](https://www.buraksenyurt.com/category/python) ile ilgili çalışmalarıma aralıklarla da olsa devam etmeye çalışıyorum. Malum iş yoğunluğu ve sıkışık proje takvimi nedeniyle eskisi kadar vakit ayıramıyorum. Bu nedenle öğrenmek istediğim konuları önceliklendirme yoluna gittim (Kendi çalışma programımda bir backlog oluşturup öğeleri önceliklendirdiğimi ifade edebilirim) Genellikle backend tarafında programlama yaptığımdan olsan gerek listenin üst sıralarında servis tabanlı geliştirmeler yer alıyor. Onlardan birisi de REST tabanlı servis uyarlamaları.

![SeptemberFall-long-goodbye.gif](/assets/images/2016/SeptemberFall-long-goodbye.gif)

İşte Eylül ayındaki önceliğimi bu konuya verdim. Tabii Eylül denince akla sonbahar, şiir, hafif hafif serinleyen hava, şöyle güzelce sarındığımız yorgan, açılan okullar, okul malzemeleri ile dolup taşan kırtasiyeler, sararan yapraklar, eğer eğlenceli geçtiyse sıcak yaz akşamlarının geride kalması nedeniyle oluşan hüzün ve benzeri şeyler geliyor.

Son yılların en popüler uygulama tiplerinden birisi REST (Representational State Transfer) tabanlı servisler. Özellikle MicroService dünyasını da ilgilendiren bu uygulama çeşidini.Net'ten Ruby'ye, Java'dan Python'a kadar pek çok programlama dilinde ve platformda ele almak mümkün. Prensipler genel olarak aynı. HTTP protokolünün Get,Post,Put,Delete gibi metodları ile çalışan, istemci tarafında Proxy tipleri gibi unsurlara ihtiyaç duymayan, basit bir tarayıcının çağırabileceği operasyonlar bütününü içeren mimari bir yaklaşım ve standart olması REST'i popüler kılan etkenler olarak düşünülebilir. Aşağıdaki tabloda HTTP metodları ile ilişkili özet bilgiyi bulabilirsiniz.

HTTP Metod
Açıklama
Örnek

GET
Bir kaynak (resource) ile ilgili bilgi almamızı sağlar. Örneğin tüm ürün listesinin çekilmesi.
http (s)://hostname/api/products

GET
Bir kaynak ile ilgili bilgi alırken listenin belirli bir tekil elemanını da çekebiliriz. Örneğin ürün listesinden 100456 nolu olanın çekilmesi gibi.
http (s)://hostname/api/products/100456

POST
Yeni bir kaynak oluşturmamızı sağlar. Kaynak içeriği HTTP paketinde gönderilir.
http (s)://hostname/api/products

PUT
Bir kaynağı güncellemek için kullanabiliriz. Talep belirli bir Id için yapılırken güncelleme içeriği yine paket içinde gönderilir.
http (s)://hostname/api/products/100456

DELETE
Bir kaynağı silmek için kullanılır. Kaynak yine tanımlayıcı bir değer ile ayırt edilir. Ürünün benzersiz IDsi gibi.
http (s)://hostname/api/products/100456

> Vakti zamanında banka içinde kullanılan yaklaşık 15 yaşındaki klasik asp ile yazılmış bir uygulamanın ihtiyaç duyduğu iş fonksiyonelliğini içeren bir Endpoint için, REST tabanlı servis geliştirmiştik. Bu konu ile ilişkili olarak ["Klasik ASP sayfasından REST Servis Çağırmak"](https://www.buraksenyurt.com/post/klasik-asp-sayfasindan-rest-servis-cagirmak.aspx) isimli makaleyi incleyebilirsiniz. Zaten servis dünyasının en güzel özelliklerinden birisi de bu; platformlar arası sınırları ortadan kaldırması.

Bu yazımızda ise Python programlama dilini kullanarak basit bir REST servisinin nasıl geliştirilebileceğini incelemeye çalışacağız. REST tipinden servisleri Python tarafında daha kolay ele almak için Flask isimli Web Framework'ünden yararlanacağız. Uygulamamızı Linux Ubuntu işletim sistemi üzerinde geliştireceğimizi baştan belirteyim (.Net platformunda geliştirme yapıyoruz diye Linux'ü bir kenara bırakacak değiliz) Öncelikle sistemin güncellenmesinde yarar var. Bunun için terminalden

sudo apt-get update

komutunun verilmesi yeterlidir. Sistem güncellemesini takiben pip aracı kullanılarak (ki pip ile Python modüllerini sistemimize yükleyip kaldırabiliyoruz) Flask kütüphanesinin yüklenmesi gerçekleştirilebilir. Terminalden aşağıdaki komutu ile işlemlerimize devam edebiliriz.

pip install flask

## Örnek Kod

Bu işlemlerin ardından flask kütüphanesini kullanacağımız örnek kodları geliştirmeye başlayabiliriz. Amacımız temel olarak bir REST servisini ayağa kaldırmak olduğundan çok yalın bir örnek üzerinden ilerleyeceğiz. Bellek üzerinde çeşitli tipte ürünlerimiz mevcut olacak (ki bir gerçek hayat senaryosunda kaynaklar çoğunlukla veritabanı üzerinden beslenecektir) Bu ürün listesi üzerinde GET, POST ve DELETE metodlarını deneyeceğiz. Servis üzerinde tüm ürün listesini çekebileceğiz. Yine belli bir ID'ye ait ürünü de alabileceğiz. Bunlara ek olarak yeni bir ürünün eklenmesi ve silinmesi operasyonlarını da deneyeceğiz. İşte örnek Python kod dosyamız.

```bash
#!flask/bin/python
from flask import Flask, jsonify
from flask import make_response
from flask import request

products = [
    {
        'id': 1000,
        'title': 'Stabilo kalem seti',
        'description': '16li renk paketi', 
        'price': 50,
        'category':'Kirtasiye',
        'inStock':True
    },
    {
        'id': 1002,
        'title': 'Python Programming',
        'description': 'Python programlama ile ilgili giris seviye kitap', 
        'price': 60,
        'category':'Kitap',
        'inStcok':False
    },
    {
        'id': 1003,
        'title': 'Mini iPod',
        'description': '80 Gb Kapasiteli MP3 Calar', 
        'price': 200,
        'category':'Elektronik',
        'inStock':True
    }
]

app = Flask(__name__)

@app.route('/azon/api/products', methods=['GET'])
def get_products():
    return jsonify({'products': products})

@app.route('/azon/api/products/<int:product_id>', methods=['GET'])
def get_product(product_id):
    product = [product for product in products if product['id'] == product_id]
    if len(product) == 0:
        return jsonify({'product': 'Not found'}),404
    return jsonify({'product': product})

@app.route('/azon/api/products', methods=['POST'])
def create_product():
    newProduct = {
        'id': products[-1]['id'] + 1,
        'title': request.json['title'],
        'description': request.json['description'],
        'price':request.json.get('price', 1),
        'category':request.json['category'],
        'inStock': request.json.get('inStock', False)
    }
    products.append(newProduct)
    return jsonify({'product': newProduct}), 201

@app.route('/azon/api/products/<int:product_id>', methods=['DELETE'])
def delete_product(product_id):
    product = [product for product in products if product['id'] == product_id]
    if len(product) == 0:
        return jsonify({'product': 'Not found'}), 404
    products.remove(product[0])
    return jsonify({'result': True})

@app.errorhandler(404)
def not_found(error):
    return make_response(jsonify({'HTTP 404 Error': 'The content you looks for does not exist. Please check your request.'}), 404)

if __name__ == '__main__':
    app.run(debug=True)#!flask/bin/python
```

Kodumuzda neler yaptığımıza kısaca değinelim dilerseniz. Uygulama, products isimli bir listeyi kullanıyor. GET, POST ve DELETE taleplerini nasıl karşılayacağımızı @app.route nitelikleri ile metodlarımızın başında belirtiyoruz. Örneğin get_products metodu başında belirtilen nitelik içeriğine göre http://localhost:5000/azon/api/products adresine gelecek olan GET taleplerini ele alacak (Yeri gelmişken Flask'in varsayılan olarak localhost üzerinden 5000 numaralı portu kullanıma sunduğunu belirtelim) Bir diğer fonksiyon olan get_product metodu ise product_id değerini alarak çalışıyor. HTTP yönlendirmesi yine @app.route bildirimi ile gerçekleştirilmekte. Buna göre http://localhost:5000/azon/api/products/1002 gibi bir çağrı ilgili get_product metodu tarafından ele alınacak.

create_product metodu HTTP Post çağrılarına cevap verecek şekilde çalışıyor. Metod içerisinde POST paketi ile gelen JSON içeriğindeki key:value değerleri kullanılarak, bellekte tutulan products listesine yeni bir ürünün eklenmesi sağlanmakta. Tahmin edileceği üzere delete_product metodu, HTTP Delete çağrısı ile gelen ürün numarasına göre işlem yapmakta ve ilgili ürünü listeden çıkartmakta (Tabii bulursa) Çok doğal olarak hatalı bir URL talebi de gönderilebilir. Buna göre oluşacak HTTP 404 hatasını da özel olarak ele almaktayız. @app.errorhandler (404) niteliği ile işaretlenmiş olan not_found fonksiyonu bu işi ele almakta. Bir nevi sunucu üzerinde oluşan hata mesajı istemci tarafına gönderilmeden önce araya girdiğimizi düşünebiliriz.

HTTP talepleri ve karşılığında çalışan komutları aşağıdaki özet tabloya bakarak daha kolay anlayabiliriz.

http://localhost:5000/azon/api/products
HTTP Get
@app.route ('/azon/api/products', methods=['GET'])
get_products

http://localhost:5000/azon/api/products/1002
HTTP Get
@app.route ('/azon/api/products/', methods=['GET'])
get_product

http://localhost:5000/azon/api/products

HTTP
Post

@app.route ('/azon/api/products', methods=['POST'])
create_product

http://localhost:5000/azon/api/products/1004

HTTP Delete

@app.route ('/azon/api/products/', methods=['DELETE'])
delete_product

Talep
Türü
Nitelik
Method

## Testler

Şimdi testlerimize başlayabiliriz. İlk olarak tüm üsrün listesini ve tek bir ürünü elde etmeye çalışalım. AzonHost.py ismiyle kaydettiğimiz kod dosyamızı pyhton yorumlayıcısı ile çalıştırdıktan sonra basit bir tarayıcı üzerinden bu talepleri aşağıdaki gibi gerçekleştirebiliriz.

http://localhost:5000/azon/api/products talebi sonrası durum aşağıdaki ekran görüntüsündeki gibi olacaktır.

![RestPython1.gif](/assets/images/2016/RestPython1.gif)

Görüldüğü gibi tüm ürün listesi JSON formatında istemciye döndürülmüştür. Dilersek tek bir ürünü talep etmeyi de deneyebiliriz. Tek yapmamız gereken aşağıdaki URL'de olduğu gibi doğru Id değerini kullanmaktır.

http://localhost:5000/azon/api/products/1002 sonrası durum aşağıdaki gibi olacaktır.

![RestPython3.gif](/assets/images/2016/RestPython3.gif)

Eğer hatalı bir URL talebi gönderip HTTP 404 hatası oluşmasına neden olursak karşımıza aşağıdaki gibi bir mesaj çıkacaktır. Nitekim HTTP 404 vakasını not_found metodu ile yakalayıp kendi istediğimiz şekilde ele almaktayız.

![RestPython2.gif](/assets/images/2016/RestPython2.gif)

Yeni bir ürün eklemek için tarayıcı yerine curl komut satırı aracını da kullanabiliriz (aslında bu servisi host ettiğiniz makineye erişebilen herhangi bir istemci uygulamayı geliştirmeyi deneyebilirsiniz..Net tarafında kuvvetli iseniz söz konusu HTTP taleplerini C# ile oluşturmaya çalışabilirsiniz. Ben söyledim siz deneyin) JSON içeriğini oluştururken string bazlı türler için çift tırnak kullanımına dikkat etmemiz gerekmekte. Ben yazarken epey hata yaptım ve 7nci denememde HTTP Post paketini ancak gönderebildim. Örnekteki price ve inStock değerleri sayısal ve boolean tipinden olduğuncan çift tırnaklar arasında yazılmamışlardır.

![RestPython4.gif](/assets/images/2016/RestPython4.gif)

curl komutunu çalıştırdıktan sonra ilgili ürünün bellekteki listeye eklendiğini görebiliriz. Delete komutu ile silme işlemini yine curl aracını kullanarak denememiz mümkün. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![RestPython5.gif](/assets/images/2016/RestPython5.gif)

1004 numaralı ürünü HTTP Delete komutu ile listemizden çıkarttık.

Pek tabii istemci olarak curl veya Firefox gibi bir tarayıcıyı kullanmak zorunda değiliz. Elimizde REST tabanlı çalışan bir servis var. Dolayısıyla istemci tarafı Java,.Net, Ruby vb bir dille yazılmış herhangi bir uygulama olabilir. Diğer yandan örneğimizde eksik bıraktığımız bir operasyon da mutlaka dikkatinizi çekmiştir; HTTP Put. Yani Update işlemimiz henüz operasyonel olarak etkin değil. Operasyon tahmin edeceğiniz üzere paket içerisinde gelecek olan bilgileri var olan bir ürününkü ile değiştirmeli. Peki ama nasıl? Makaleme son verirken bu kutsal görevi de siz değerli okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
