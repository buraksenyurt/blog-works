---
layout: post
title: "BeeWare ile Linux Platformunda Desktop Uygulaması Geliştirmek ve Android Sürümünü Oluşturmak"
date: 2020-09-13 21:05:00 +0300
categories:
  - python
tags:
  - pyhton
  - cross-platform-development
  - widget
  - briefcase
  - android
  - emulator
  - togo
---
Geçenlerde Python ile ilgili bir şeyler ararken BeeWare isimli çalışmaya rastladım. Her yerde python ile native uygulama geliştirmek gibi bir felsefesi vardı. Eee zaten python her platformda yüklenip kullanılmıyor mu? Yoo tam olarak olay öyle değil aslında. BeeWare ürünü macOS, Linux ve Windows platformlarında native uygulama geliştirmek haricinde iOS ve Android için de destek sunan bir araçlar ve kütüphaneler topluluğu. Söz gelimi iOS ve macOS platformundaki Objective C kütüphaneleri ile Python arasında köprü görevi gören Rubicon ObjC isimli bir araç sunuyor. Java kütüphaneleri ile bir iletişim mi gerekiyor!? O zaman Rubicon Java var. Diğer yandan cross-platform için Toga isimli bir widget kütüphanesi kullanıyor. Ayrıca python projelerini tek başına çalışabilir uygulamalar haline getirmek için (standalone native application) Briefcase isimli başka bir araca sahip ki bir çoğunu birazdan kullanacağız.

![beeware.png](/assets/images/2020/beeware.png)

Aslında olayı şöyle düşünebiliriz; Bu çatı sayesinde Android için Gradle çıktısı, iOS için XCode proje çıktısı, Linux için AppImage, Windows için MSI Installer ve macOS için doğrudan çalışabilir uygulama çıktıları üretebiliyoruz. Bana Xamarin ve Electron'u düşündürmedi değil (Bu arada BeeWare'in Logo'su epey sevimli)

İddialı bir platform olduğunu ifade edebilirim. Elbette Skynet çalışmasını yaptığım ve derlemeyi hazırladığım tarih itibariyle ürünle ilgili değişiklikler olmuştur. O nedenle [şu adresten takip etmenizi](https://beeware.org/) öneririm. Benim amacım her zaman olduğu gibi bunu Heimdal (Ubuntu-20.04) üzerinde denemek ve Linux'te çalışan bir masaüstü uygulaması geliştirmek. Hatta arkasına birde Android sürümünü eklemek. İşe uzun bir terminal çalışması ile başlıyoruz.

```bash
# Sistemde Python yüklü olsa bile ekstra bazı kütüphaneler de gerekiyor
# Lakin bu paketleri hangi amaçla yüklüyoruz, araştırmam lazım. Mazallah güvenlik açığı filan da olabilir. Aman dikkat!
sudo apt-get install libgirepository1.0-dev libcairo2-dev libpango1.0-dev libwebkit2gtk-4.0.37 gir1.2-webkit2-4.0

# Şimdi Python paketinin dağıtımında devreye girecek Briefcase aracını yükleyelim
# Bu arada 20.04 üstünde cookiecutter versiyonunu beğenmedi Heimdall. O nedenle cookiecuttor'ı da pip üstünden install ettim
python3 -m pip install briefcase

# Adettendir kurulan versiyonu bir kontrol etmek iyi olabilir
briefcase --version

# Şimdi yeni projenin açılışını yapabiliriz
briefcase new

# Sorulan sorulara verdiğim cevaplar doğrultusunda cardgame isimli bir proje oluştu. 
# Carg Game isimli projenin GUI framework olarak Toga'yı seçtim. 
# Buna göre projenin Linux, macOS, Windows dağıtımlarındaki gereksinimleri ile birlikte
# diğer sorduğu sorulara() verdiğim cevaplar pyproject.toml (toml = Tom's Obvious Minimal Language) içerisine yazıldı.
# Bu dosyayı incelemekte yarar var.
```

![skynet_29_Screenshot_01.png](/assets/images/2020/skynet_29_Screenshot_01.png)

Bir yerlere kod yazmayacak mıyız dediğinizi duyar gibiyim? Sonuçta bir arayüz tasarlamak gerekiyor öyle değil mi? Bunu src/cardgame altındaki app.py dosyasında yapabiliriz. Aynen aşağıdaki kod parçasında olduğu gibi.

```text
"""
A simple desktop application on Ubuntu
"""
import toga
from toga.style import Pack
from toga.style.pack import COLUMN, ROW

class CardGame(toga.App):

    def startup(self):

        # Bu ana form gibi bir şey. Toga uygulamalarında kutu koleksiyonları da söz konusu olabilir(Box Collection)
        # direction için verdiğim Column ile main_box'a eklenecek kontrollerin sütun formatında aşağıya doğru ineceğini söyledik
        main_box = toga.Box(style=Pack(direction=COLUMN))

        # Şimdi birkaç kontrol ekleyelim
        # Birkaç Label ekliyoruz ve onlara padding stilleri veriyoruz
        lblNickname = toga.Label(
            'Takma adını söyler misin?', style=Pack(padding=(0, 10)))  # Biraz padding ayarı yaptık
        lblColor = toga.Label('En sevdiğin renk?', style=Pack(padding=(0, 10)))
        lblLuckyNumber = toga.Label(
            'Şans numaran peki?', style=Pack(padding=(0, 10)))
        self.lblMyGuess = toga.Label('...', style=Pack(flex=1, padding=10))

        # Şimdi yukarıdaki sorular için birer Input kontrolü ekleyelim
        # Bunları sınıfın Instance Variable'ları olarak tanımlıyoruz ki erişmemiz kolay olsun.
        self.txtNickname = toga.TextInput(style=Pack(padding=5, flex=1))
        self.txtColor = toga.TextInput()
        self.txtLuckyNumber = toga.TextInput()

        # Şimdi yukarıdaki kontrolleri bir kutuya koyalım. (Box)
        boxIdentity = toga.Box(style=Pack(direction=COLUMN))
        boxIdentity.add(lblNickname)
        boxIdentity.add(self.txtNickname)
        boxIdentity.add(lblColor)
        boxIdentity.add(self.txtColor)
        boxIdentity.add(lblLuckyNumber)
        boxIdentity.add(self.txtLuckyNumber)

        # Sonra bu kontrolleri içeren kutuyu ana kutuya ekleyelim
        main_box.add(boxIdentity)

        # Bir tane de Button oluşturup ana kutuya ilave edelim
        # Button'a basıldığında da bntGuess_OnPress isimli olay metodu çalışacak
        btnGuess = toga.Button(
            'Tahmin Yap', on_press=self.btnGuess_OnPress, style=Pack(width=100, padding=10))
        main_box.add(btnGuess)

        main_box.add(self.lblMyGuess)

        # Title ile biraz oynadım. Keh keh keh :P
        self.main_window = toga.MainWindow(
            title=self.formal_name+" Version 1.0")
        self.main_window.content = main_box
        self.main_window.show()

        # Windows Forms tarafına ne kadar da benziyor
    def btnGuess_OnPress(self, widget):
        print('Belki loglama amaçlı kullanılabilir')
        # Burada validasyon yapmak gerekir mi? Yoksa kontrollerin validasyon için nitelikleri var mıdır?
        summary = "Merhaba {nick}. Favori rengin {color} ve şans numaran {number}".format(
            nick=self.txtNickname.value, color=self.txtColor.value, number=self.txtLuckyNumber.value)
        self.lblMyGuess.text = summary

'''
    Uygulama yüklendiğinde main metodu CardGame sınıfını örnekler
    Bunu bir Windows Forms sınıfının yüklenmesi olarak düşünüyorum ;)
'''

def main():
    return CardGame()
```

Aslında Windows Forms veya Asp.Net Web Forms ile kodlama yaptıysanız buradaki kontrollerin yerleşimleri ile event'leri anlamanız son derece kolay. Gelelim çalışma zamanına. Linux, MacOS, Windows...Hiç farketmez. Hepsinde briefcase aracını kullanarak uygulamayı işletebiliriz.

```bash
briefcase dev
```

İlk çalışma sırasında üzerinde olduğumuz platforma göre gerekli bazı bağımlılıklar indirilir (Toga paketleri gibi)

![skynet_29_Screenshot_02.png](/assets/images/2020/skynet_29_Screenshot_02.png)

Ve ardından uygulama aşağıdaki ekran görüntüsünde olduğu gibi ayağa kalkar.

![skynet_29_Screenshot_03.png](/assets/images/2020/skynet_29_Screenshot_03.png)

## Dağıtım (Deployment)

Uygulama şu ana kadar development modda çalıştırıldı. Ancak onu paket haline getirip farklı platformlara da dağıtabiliriz. Ben normalde Linux için bir dağıtım paketi oluşturmayı düşünüyordum ancak uygulamayı Android için paketleyebilir miyiz diye de merak ettim. İşte uygulamanın android sürümüne dönüştürülmesi için yapmamız gerekenler.

```bash
# Uygulama klasöründeyken create ile bir android app oluşturulur(dakikalarca sürebiliyor)
briefcase create android
# ve build komutu ile de apk dosyası üretilir. Card Game/app/build/outputs/apk/debug/app-debug.apk altında oluşur (Gerekli SDK, NDK paketlerini indirdiği için ilk seferinde dakikalarca sürebiliyor)
briefcase build android

# Kontrol amaçlı olarak build edilen sürüm aşağıdaki gibi çalıştırılabilir.
# Burada sanal bir emülator'den yararlanılabileceği gibi gerçek bir Android cihazda kullanılabilir.
# Ben Create a new Android Emulator seçeneğini tercih ettim.
briefcase run android
```

Ve bende tebessüm bırakan ekran görüntüsü:-)

![skynet_29_Screenshot_04.png](/assets/images/2020/skynet_29_Screenshot_04.png)

Bu örnek için andorid klasörü tüm bağımlılıkları ile birlikte 500 megabyte'tan fazla yer tuttu. APK dosyası ise yaklaşık 50 Mb civarındaydı. Böylesine basit bir kobay uygulama için oldukça fazla yer harcandığını ifade edebilirim. Zaten genel olarak bu tip cross-platform çözümlerinin (Xamarin, Electron vb) özellikle mobil taraftaki en büyük sorunu da sanırım bu optimizasyo konusundaki sıkıntıları.

Peki örnekle ilgili daha neler yapabilirsiniz? Söz gelimi verileri SQLite gibi bir fiziki bir depolama alanında saklamayı deneyebilirsiniz (Uzak bir REST servis üstünden de olabilir tabi) Ayrıca elinizin altında bir macOS varsa iOS sürümü için bir dağıtım paketi üretip o platformda kullanmayı da düşünebilirsiniz. Uygulamamızın örnek kodlarına SkyNet [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2029%20-%20What%20is%20BeeWare) erişebilirsiniz. Tekrardan görüşünceye dek hepinize iyi günler dilerim.
