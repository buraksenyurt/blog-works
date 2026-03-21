---
layout: post
title: "Python - Flask ile Basit Bir Web Uygulaması Geliştirmek"
date: 2017-08-09 21:02:00 +0300
categories:
  - python
tags:
  - python
  - flask
  - web-programlama
  - http-post
  - post
  - jinja
  - http
  - html
  - template
---
Yazıyı yazdığım şu yaz gününde hava epey sıcak. İstanbul'da öğle saatlerinde 39 dereceyi gördük. Güney tarafında yaşayan bir kaç yakın arkadaşımdan 48 dereceli rakamları duyduktan sonra ise halimize şükredelim dedim. Açtım Python kitabımı, çalışmaya devam ettim.

![sicak.jpg](/assets/images/2017/sicak.jpg)

Bir süre önce GoLang tarafında basit web uygulamalarının nasıl geliştirilebileceğini incelemeye çalışmıştım. Daha önceden de Python tarafında Flask paketinden yararlanarak [REST tabanlı bir servis](https://www.buraksenyurt.com/post/python-ile-rest-tabanli-servis-gelistirmek) geliştirmeyi denemiştim. Tabii servis bir yanaaaa web uygulamaları bir yana. Son kullanıcı çoğunlukla görsel bir şeyler bekliyor. Python camiasında Djiango Framework bu anlamda daha popüler tabii ama henüz onu inceleme fırsatım olmadı. Flask oldukça hafif bir framework olarak karşımıza çıkmakta. Bende ondan faydalanarak basit bir web uygulaması nasıl yapılabilir inceleyeyim dedim.

Yazımızın ilerleyen kısımlarında Flask paketi ve template tipinden HTML şablonlarından yararlanarak oldukça basit bir Web uygulaması geliştirmeye çalışacağız. Yazacağımız web uygulaması temel olarak iki sayının toplamını hesaplayan bir arabirim sunacak ancak öğreneceğimiz temel esaslar daha gelişmiş web uygulamalarının tasarlanmasında da kullanılmakta. Özellikle web sayfası içeriği ve Python tarafında web sunucu görevini üstlenecek kodlar ile arada kurmaya çalışacağımı bir iletişim söz konusu olacak. Dilerseniz vakit kaybetmeden uygulamamızı geliştirmeye başlayalım.

Klasör Yapısı

İşe ilk olarak klasör yapısını anlatarak başlamalıyım. Aşağıdaki kurguyu ele alabiliriz.

/hello_flask.py
/templates/
basepage.html
einstein.html
result.html
/static/
main.css

hello_flask isimli python sayfamızda http taleplerini yönledirme işlemlerini gerçekleştireceğiz. templates klasöründe bazı basit HTML içerikleri yer alıyor. Bunlar şablon sayfalarımız olarak düşünülebilirler. basepage diğerleri için ata şablon görevini üstlenmekte. Einstein.html içerisinde toplama işlemi için bir içerik sunarken işlem sonuçlarını result.html sayfasından sunmayı planlıyoruz. Tasarldığımız kurguda önemli olan noktalardan birisi HTML sayfaları ile Flask çatısının iletişimi. Şimdi şablon sayfalarını yakından tanımaya çalışalım.

Template Sayfaları

Web uygulamamızda [Jinja2](http://jinja.pocoo.org/docs/2.9/) standartlarında içerik sunacak HTML sayfaları bulunuyor. Bu HTML sayfaları Flask tarafı ile iletişim halinde olacak. Aslında olay &#123;&#123; ve &#125;&#125; arasındaki kısımlarda gerçekleşmekte. Burada kullanılan değişken adları python tarafında da değerlendirilebiliyor. Bir başka deyişe python kod tarafı ile statik HTML sayfaları arasındaki veri alışverişinde bu söz dizimi değer bulacak.

## basepage.html

BasePage.html.Net tarafında Web uygulaması geliştiren arkadaşlarımızca Master Page olarak düşünebilir. Kısaca diğer sayfalar için tepede yer alan bir ana şablon vazifesi görmekte.

{% raw %}
```text
<!doctype html>
<html>
    <head>
        <title>{{ page_title }}</title>
        <link rel="stylesheet" href="static/main.css"/>
    </head>
    <body>
        {% block body %}
        {% endblock %}
    </body>
</html>
```
{% endraw %}

Dikkat edileceği üzere title elementinde page_title, body kısmında ise block body ve endblock isimli tanımlamalar mevcut. Bu tanımlamaların &#123;&#123; ve &#125;&#125; arasında olduklarına dikkat edelim. Alt sayfaların block body ve endblock isimli kısımlar içerisine yerleşeceğini de söyleyebiliriz.

## einstein.html

Base Page'den türeyen Einstein.html temel bir toplama operasyonunu üstlenmekte. İlk olarak extends isimli bir tanımlama ile başladığını görebiliriz. Burada basepage.html'den türetildiğini belirtiyoruz. block body ve endblock kısımları arasında bir takım tanımlamalar mevcut. h2 boyutlarında bir başlık belirttikten sonra POST metodunu kullanan bir form yer alıyor.

{% raw %}
```json
{% extends 'basepage.html' %}
{% block body %}

<h2>{{ page_title }}</h2>

<form method='POST' action='/sum'>
<table>
<p>Sum of two values</p>
<tr><td>First Value:</td><td><input name="firstValue" type="TEXT" width="10"</td></tr>
<tr><td>Second Value:</td><td><input name="secondValue" type="TEXT" width="10"</td></tr>
</table>
<p><input value="Calculate" type="SUBMIT"></p>
</form>

{% endblock %}
```
{% endraw %}

Pek tabii form elementinin method ve action niteliklerine atanan değerler oldukça kıymetli. Calculate isimli butona basıldığında gerçekleşecek Submit işlemi sonrası [http://localhost:5000/sum](http://localhost:5000/sum) adresine gidilecek. Bu işlem HTTP protokolünün POST metoduna göre gerçekleşecek. Form üzerinde iki tane text kontrolü var. Bunlar toplama işlemine dahil edilecek değişkenleri aldığımız kontroller.

## result.html

Toplama işleminin sonucunu göstereceğimiz HTML şablonu ise aşağıdaki içeriğe sahip.

{% raw %}
```json
{% extends "basepage.html" %}
{% block body %}

<h2>{{ page_title }}</h2>
<p>You submitted the following data:</p>
<table>
    <tr><td>First:</td><td>{{ first_value }}</td></tr>
    <tr><td>Second:</td><td>{{ second_value }}</td></tr>
</table>

<p>Result</p>
<h3>{{ sum_result }}</h3>

{% endblock %}
```
{% endraw %}

Yine basepage sayfasından yapılan bir genişletme olduğunu ifade edebiliriz. Gövde bu kez sonuçları göstereceğimiz HTML elementlerini barındırıyor. table elementi içerisinde first_value, second_value ve sonrasında gelen sum_result isimli değişklenlerle toplama işlemine ait detayları ve sonucu gösteriyoruz. Tüm değişkenlerin Jinja'nın istediği şekilde &#123;&#123; ve &#125;&#125; arasında yazıldığına dikkat edelim. Benzer yaklaşım GoLang tarafında da mevcuttu.

## hello_plask.py

Aslında en kilit nokta bu pyhton kod dosyası diyebiliriz. Flask ile entegre çalışan bu kod parçası, [http://localhost:5000](http://localhost:5000) nolu porta gelecek talepleri değerlendirmek üzere çalışmakta.

```text
from flask import Flask, render_template,request
app=Flask(__name__)

@app.route('/')
def entry_page()->'html':
    return render_template('einstein.html',page_title='Wellcome to Little Einstein Project')

@app.route('/sum',methods=['POST'])
def sum()->'html':
    x=int(request.form['firstValue'])
    y=int(request.form['secondValue'])
    return render_template('result.html',page_title='Calculation result',sum_result=(x+y),first_value=x,second_value=y,)

app.run(debug=True)
```

İlk olarak Flask, render_template ve request modüllerinin entegre edildiğini belirtelim. @app.route ile başlayan decorator tanımlamaları ile takip eden fonksiyonların çalışma zamanlarındaki davranışlarını değiştirmekteyiz. Buna göre / adresine gelecek talepler entry_page, /sum adresine gelecek olan talepler ise sum fonksiyonu tarafından değerlendirilecek.

Kullanıcı einstein.html sayfasındaki butona bastığında POST metodu ile /sum adresine doğru bir yönlendirme yapılmakta. Bu kısım sum fonksiyonunca ele alınıyor. app.run (debuy=True) ifadesine göre çalışma zamanındaki tüm işlemleri (HTTP talepleri, olası çalışma zamanı hataları vb) konsolda görmemiz mümkün.

entry_page ve sum metodlarının çıktıları html olacak şekilde belirtildi. İşte bu noktada tasarlanan HTML sayfalarının render edilmesi ve istemciye gönderilmesi söz konusu. render_template metodu bu noktada devreye girmekte. İlk parametre ile hangi HTML içeriğini çağıracağımızı belirtiyoruz. Bu içerikler otomatik olarak templates klasöründe aranacaklar. Metod çağrısında takip eden parametreler ise tahmin edeceğiniz üzere render edilen HTML içerisinde kullanılan değişken adları.

Çalışma Zamanı

Yazdığımız uygulamayı IDLE üzerinden değil de komut satırından çalıştırmamız çok daha doğru olacaktır. Web sayfalarına gelecek olan talepler debug=True ataması nedeniyle komut satırına yansıtılacak. Bu sayede anlık olarak gelişmeleri takip edebiliriz. Aşağıdaki ekran görüntüsünde bunun bir örneğini görebiliriz.

![flaskweb_3.gif](/assets/images/2017/flaskweb_3.gif)

İlk olarak root adrese talepte bulunalım. Aşağıdaki çıktı ile karşılamamız gerekiyor. (Burada kullanılan CSS'i [kitabın önerdiği adresten](http://python.itcarlow.ie/ed2/ch05/static/hf.css) kullandım. Sadece ufak değişikliklerim oldu. Siz farklı stiller uygulayabilir ve web içeriğini çok daha şık hale getirebilirsiniz)

![flaskweb_1.gif](/assets/images/2017/flaskweb_1.gif)

[http://localhost:5000](http://localhost:5000)/ adresine yapacağımız talebin karşılığında hello_flask.py içerisindeki entry_page metodu devreye girecek ve einstein.html isimli sayfanın istemciye sunulması sağlanacaktır. Gelen ekrandaki kontrollere iki sayısal değer girip Calculate butonuna basarsak Post işlemi sonrası Sum isimli operasyon çalıştırılacak ve aşağıdaki çıktı elde edilecektir.

![flaskweb_2.gif](/assets/images/2017/flaskweb_2.gif)

Görüldüğü üzere Flask paketini kullanarak Python tarafında bir web uygulaması geliştirmek ve şablon HTML sayfalarını kullanmak oldukça basit. Pek tabii şablon kullanımında &#123;&#123; ile &#125;&#125; arasına alınabilecek çok farklı teknikler de söz konusudur. Bunları kitabın ilerleyen kısımlarında bulabileceğimi düşünüyorum. Öğrendikçe sizlerle paylaşmaya çalışacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
