using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueCreator
{
    [MenuItem("Tools/Create Dialogue Assets")]
    static void CreateAll()
    {
        string root = "Assets/Resources/Dialogues";
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(root))
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");

        var n = new Dictionary<string, DialogueNode>();

        string[] names = {
            // PATRON
            "Patron_Intro", "Zebercet_PatronReply", "Patron_Outro",

            // ERNEST PHASE 1 (ilk tanisma) — startNode: Zebercet_ErnestGreet
            "Zebercet_ErnestGreet", "Ernest_Meet1", "Zebercet_ErnestReply1", "Ernest_Meet2",

            // ERNEST PHASE 2 (murekkep) — startNode: Ernest_Ink1
            "Ernest_Ink1", "Zebercet_InkChoice1", "Ernest_Ink2", "Zebercet_Ink2",
            "Ernest_Ink3", "Zebercet_InkChoice2", "Ernest_KalsinMadem",
            "Zebercet_SpotErnest", "Ernest_CaughtExcuse", "Zebercet_CaughtChoice",
            "Patron_Call1", "Zebercet_PatronCall1", "Patron_Call2",

            // DOLORES PHASE 1 (ilk tanisma) — startNode: Dolores_Meet1
            "Dolores_Meet1", "Zebercet_DoloresMeet1", "Dolores_Meet2",
            "Zebercet_DoloresMeet2", "Dolores_Meet3",

            // DOLORES PHASE 2 (temizlik) — startNode: Zebercet_DoloresAsk
            "Zebercet_DoloresAsk", "Dolores_Clean1", "Zebercet_DoloresClean1",
            "Dolores_Clean2", "Zebercet_DoloresClean2", "Dolores_Clean3", "Zebercet_DoloresClean3",
            "Dolores_Delivery",

            // DORIS PHASE 1 (giris) — startNode: Zebercet_DorisMeet
            "Zebercet_DorisMeet", "Doris_Meet1", "Gilbert_Shy", "Doris_Meet2", "Zebercet_DorisMeet2",

            // DORIS PHASE 2 (ilac) — startNode: Zebercet_DorisAsk
            "Zebercet_DorisAsk", "Doris_Sick1", "Zebercet_DorisEncourage", "Doris_Sick2",
            "Zebercet_DorisOffer", "Doris_Sick3", "Zebercet_IlacChoice", "Zebercet_IlacGosBuy", "Doris_Finale",

            // NARRATION — siyah ekran gecis ve son nodeları
            "Narration_InkDone", "Narration_ErnestSneaks", "Narration_ErnestExpelled",
            "Narration_PatronHangup", "Narration_BadEnding1", "Narration_BadEnding2", "Narration_BadEnding3"
        };

        foreach (var name in names)
        {
            var existing = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{root}/{name}.asset");
            if (existing != null) { n[name] = existing; continue; }
            var node = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(node, $"{root}/{name}.asset");
            n[name] = node;
        }

        // ── PATRON ──────────────────────────────────────────────────────────
        Set(n["Patron_Intro"], "Patron",
            "Hoş geldin. Ben bu otelin patronuyum, sen Zebercet olmalısın. Bu otelin bazı kuralları vardır. Her misafirimiz başına prim para alırsın ve onların istedikleri şeyleri sağlamalısın, tabii ki dışarıdan değil! Resepsiyonumuz aynı zamanda market, müşteriler dışarıdan ürün getirmesin diye böyle bir çözüm buldum. Nasıl ama?",
            next: n["Zebercet_PatronReply"]);

        Set(n["Zebercet_PatronReply"], "Zebercet", "", choices: new Choice[] {
            C("Muhteşem!", 0, n["Patron_Outro"]),
            C("Biraz acımasızca?", 0, n["Patron_Outro"])
        });

        Set(n["Patron_Outro"], "Patron",
            "Neyse, Baba'nın borcunu ödediğinde gidebilirsin. İyi görevler.");

        // ── ERNEST PHASE 1 ──────────────────────────────────────────────────
        Set(n["Zebercet_ErnestGreet"], "Zebercet",
            "Merhaba, ben yeni resepsiyonistinizim bir şeye ihtiyaç duyduğunuzda çekinmeden sorabilirsiniz.",
            next: n["Ernest_Meet1"]);

        Set(n["Ernest_Meet1"], "Ernest",
            "Selam, ben Ernest belki duymuşsundur lokal gazetede köşe yazarıyım. Burada yenisin demek.",
            next: n["Zebercet_ErnestReply1"]);

        Set(n["Zebercet_ErnestReply1"], "Zebercet",
            "Buralarda yeniyim, duymadım efendim.",
            next: n["Ernest_Meet2"]);

        Set(n["Ernest_Meet2"], "Ernest",
            "Genelde tanırlardı aslında… neyse kolay gelsin.");

        // ── ERNEST PHASE 2 ──────────────────────────────────────────────────
        Set(n["Ernest_Ink1"], "Ernest",
            "Yeni yazımı gördün mü bu arada gazetede ikinci sayfanın hemen arkasında.",
            next: n["Zebercet_InkChoice1"]);

        Set(n["Zebercet_InkChoice1"], "Zebercet", "", choices: new Choice[] {
            C("Hayır efendim.", 0, n["Ernest_Ink2"]),
            C("Evet çok iyi bir kaleminiz var gerçekten.", 0, n["Ernest_Ink2"])
        });

        Set(n["Ernest_Ink2"], "Ernest",
            "Neyse, o değil de kalemimin mürekkebi bitti de…",
            next: n["Zebercet_Ink2"]);

        Set(n["Zebercet_Ink2"], "Zebercet",
            "Hemen resepsiyon/marketten getiriyim efendim.",
            next: n["Ernest_Ink3"]);

        Set(n["Ernest_Ink3"], "Ernest",
            "Şey işte o biraz pahalı da acaba, dışarıdan getirir misin? Hem bahşiş de veririm!",
            next: n["Zebercet_InkChoice2"]);

        Set(n["Zebercet_InkChoice2"], "Zebercet", "", choices: new Choice[] {
            C("Patronum dışarıdan herhangi bir şey sokmamı hoş karşılamıyor efendim, kurallara aykırı resepsiyon/marketten getirebilirim isterseniz.", 0, n["Ernest_KalsinMadem"]),
            C("Aslında yasak ama kimse görmezse sıkıntı olmaz sanırım.", 150, n["Narration_InkDone"])
        });

        Set(n["Ernest_KalsinMadem"], "Ernest",
            "Öyle olsun… kalsın madem.", moneyDelta: 100, next: n["Narration_ErnestSneaks"]);

        Set(n["Zebercet_SpotErnest"], "Zebercet",
            "O elinizdeki de ne?",
            next: n["Ernest_CaughtExcuse"]);

        Set(n["Ernest_CaughtExcuse"], "Ernest",
            "Hadi ama sadece bir mürekkep bir şey kaybetmezsin içeriye alsan bizi.",
            next: n["Zebercet_CaughtChoice"]);

        Set(n["Zebercet_CaughtChoice"], "Zebercet", "", choices: new Choice[] {
            C("Yalnız efendim kural kuraldır. Sizi şikayet etmek zorundayım.", 500, n["Narration_ErnestExpelled"], eventKey: "ernest_expelled"),
            C("Tamam ama ikimiz de görmedik sayıyorum bunu bu seferlik geçin.", 0, n["Patron_Call1"])
        });

        Set(n["Patron_Call1"], "Patron",
            "Sen ne yaptığının farkında mısın?!",
            next: n["Zebercet_PatronCall1"]);

        Set(n["Zebercet_PatronCall1"], "Zebercet",
            "Pardon, Patron bir şey mi oldu?",
            next: n["Patron_Call2"]);

        Set(n["Patron_Call2"], "Patron",
            "GAZETEDE MANŞET OLMUŞUZ!?! Bir tane yazar otelim ve senin hakkında ağıza alınmayacak şeyler söylemiş! Ben bu kaybı nasıl karşılayacağım söylesene! Maaşından kesiyorum bunu uyarı olarak kabul et.",
            moneyDelta: -500, next: n["Narration_PatronHangup"]);

        // ── DOLORES PHASE 1 ─────────────────────────────────────────────────
        Set(n["Dolores_Meet1"], "Dolores",
            "Ay Merhaba çocuğum, yeni görevli sen olmalısın. Babanı çok severdim, çok efendi biriydi…",
            next: n["Zebercet_DoloresMeet1"]);

        Set(n["Zebercet_DoloresMeet1"], "Zebercet",
            "Merhabalar efendim evet benim. Babamı çok tanımadım aslında, iyi biridir herhalde.",
            next: n["Dolores_Meet2"]);

        Set(n["Dolores_Meet2"], "Dolores",
            "Öyleydi tabii ya, bana hep en iyi kalite temizlik malzemelerinden verirdi…",
            next: n["Zebercet_DoloresMeet2"]);

        Set(n["Zebercet_DoloresMeet2"], "Zebercet",
            "Odanız ondan çok temiz kokuyor olmalı. Her zaman buradayım efendim lazım olan bir şey varsa seve seve yardım ederim.",
            next: n["Dolores_Meet3"]);

        Set(n["Dolores_Meet3"], "Dolores",
            "Ah! Temizlik çok mühimdir çocuğum sakın bunu unutma. Elbet ihtiyacım olacak, iyi günler.");

        // ── DOLORES PHASE 2 ─────────────────────────────────────────────────
        Set(n["Zebercet_DoloresAsk"], "Zebercet",
            "Bir ihtiyacınız var mıdır?",
            next: n["Dolores_Clean1"]);

        Set(n["Dolores_Clean1"], "Dolores",
            "Ay merhaba çocuğum, aslında var. Uzun süredir odamı temizlemedim de yani… bir 3-4 saatir falan.",
            next: n["Zebercet_DoloresClean1"]);

        Set(n["Zebercet_DoloresClean1"], "Zebercet",
            "Odanızı hemen temizlerim efendim kusura bakmayın, hallediyorum.",
            next: n["Dolores_Clean2"]);

        Set(n["Dolores_Clean2"], "Dolores",
            "Yok yok. Yani aslında ben direkt temizlik malzemesi istemiştim de… İçime sinmiyor öbür türlüsü.",
            next: n["Zebercet_DoloresClean2"]);

        Set(n["Zebercet_DoloresClean2"], "Zebercet",
            "Hiç zahmet etmeyin hemen otelin temizlik dolabından malzemeleri alıp temizlerim efendim.",
            next: n["Dolores_Clean3"]);

        Set(n["Dolores_Clean3"], "Dolores",
            "HAYIR! Oradan olmaz yani… resepsiyon/marketten en kalitelisinden olsun çocuğum. Öbürleri çok başımı ağrıtıyor da şöyle güzelce temizlerim ben.",
            next: n["Zebercet_DoloresClean3"]);

        Set(n["Zebercet_DoloresClean3"], "Zebercet",
            "Peki efendim hemen getiriyorum.");

        Set(n["Dolores_Delivery"], "Dolores",
            "Ay! Geldiniz, ne kadar güzelsiniz! Babana çok benzediniz, çok efendi biriydi… işte paranız çocuğum.",
            moneyDelta: 500);

        // ── DORIS PHASE 1 ───────────────────────────────────────────────────
        Set(n["Zebercet_DorisMeet"], "Zebercet",
            "Otelimize hoş geldiniz, ben Zebercet. Sizinle ben ilgileneceğim, ne zaman isterseniz çağırabilirsiniz.",
            next: n["Doris_Meet1"]);

        Set(n["Doris_Meet1"], "Doris",
            "Merhaba ben Doris bu da oğlum Gilbert. Biz de memnun olduk. Değil mi Gilbert?",
            next: n["Gilbert_Shy"]);

        Set(n["Gilbert_Shy"], "Gilbert",
            "*utanma sesi*",
            next: n["Doris_Meet2"]);

        Set(n["Doris_Meet2"], "Doris",
            "Biraz huysuzdur, bütün gün ağzına bir lokma bile sürmedi de.",
            next: n["Zebercet_DorisMeet2"]);

        Set(n["Zebercet_DorisMeet2"], "Zebercet",
            "Çok şekermiş efendim.");

        // ── DORIS PHASE 2 ───────────────────────────────────────────────────
        Set(n["Zebercet_DorisAsk"], "Zebercet",
            "Sizin için ne yapabilirim efendim?",
            next: n["Doris_Sick1"]);

        Set(n["Doris_Sick1"], "Doris",
            "Selam, aslında biraz utanıyorum da…",
            next: n["Zebercet_DorisEncourage"]);

        Set(n["Zebercet_DorisEncourage"], "Zebercet",
            "Lütfen çekinmeyin size hizmet etmek benim görevim.",
            next: n["Doris_Sick2"]);

        Set(n["Doris_Sick2"], "Doris",
            "Çok öyle bir şey değil aslında söylemeye de çekiniyorum. Çocuğum Gilbert çok hasta, yolda otele gelene kadar zor dayandı. İlaç rica edecektim sizden…",
            next: n["Zebercet_DorisOffer"]);

        Set(n["Zebercet_DorisOffer"], "Zebercet",
            "Tabii ki efendim hemen getiriyorum resepsiyon/markette olması gerek.",
            next: n["Doris_Sick3"]);

        Set(n["Doris_Sick3"], "Doris",
            "Durum şu ki ilacı alacak param mevcut değil… Siz karşılasanız… çok makbule geçersiniz.",
            next: n["Zebercet_IlacChoice"]);

        Set(n["Zebercet_IlacChoice"], "Zebercet", "", choices: new Choice[] {
            C("Aslında ben de buraya babamın borcunu ödemek için geldim. Param olsa zaten burada olmam efendim. Kusura bakmayın.", 0, n["Narration_BadEnding1"]),
            C("Bunu yapabilirim galiba beni sıkıntıya sokabilir ama burada bekleyin ilacı alıp geliyorum.", 0, n["Zebercet_IlacGosBuy"])
        });

        Set(n["Zebercet_IlacGosBuy"], "Zebercet",
            "Resepsiyona gidiyorum, bir dakika bekleyin.",
            eventKey: "doris_buy_medicine");

        Set(n["Doris_Finale"], "Doris",
            "Sana bunu söylememem gerekiyor ama çok iyi bir insana benziyorsun… Ben babanın gayri resmi eşiyim. Gilbert'a yani kardeşine ilacı sağlayarak hayatta kalmasını sağladın. Ne kadar teşekkür etsem az… Ve buraya boşuna gelmedik. Baban ölmedi Zebercet. Patron babanı kaçırdı ve babana o mektubu yazdırdı. Sadece sen burada o pis patrona daha fazla para kazandır diye. Babanı ölmüş gibi gösterip sana olmayan bir borcu ödetiyor.",
            eventKey: "ending_good");

        // ── NARRATION (siyah ekran gecis ve son nodeları) ────────────────────
        Set(n["Narration_InkDone"], "",
            "Mürekkep alıp gizlice otele getirirsin. (+150 Bahşiş)");

        Set(n["Narration_ErnestSneaks"], "",
            "Yazar ana kapıdan gizlice girdiğini görürken gazeteye sarılı mürekkebi görürsün…",
            next: n["Zebercet_SpotErnest"]);

        Set(n["Narration_ErnestExpelled"], "",
            "Patron size 500 para ikram eder. ERNEST otelden kovulur.");

        Set(n["Narration_PatronHangup"], "",
            "Patron telefonu yüzüne kapar. Patron sizden 500 para alır.\n(Not: Ernest otelde kalmaya devam eder.)");

        Set(n["Narration_BadEnding1"], "",
            "Borcunu ödeyecek kadar parayı tamamladın… Eve gitmek için otobüse binecekken bir mektup gelir.",
            next: n["Narration_BadEnding2"]);

        Set(n["Narration_BadEnding2"], "",
            "Sevgili oğlum, Bu mektubu okuyorsan senden sakladığım Gilbert adında başka bir oğlumun daha olduğunu bilmeni isterim.",
            next: n["Narration_BadEnding3"]);

        Set(n["Narration_BadEnding3"], "",
            "Gilbert'ın da öldüğü haberlerini gazeteden öğrenirsin.",
            eventKey: "ending_bad");

        foreach (var kvp in n)
            EditorUtility.SetDirty(kvp.Value);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Tüm diyalog asset'leri oluşturuldu ({names.Length} node) → Resources/Dialogues/");
    }

    static void Set(DialogueNode node, string speaker, string text,
        DialogueNode next = null, Choice[] choices = null,
        int moneyDelta = 0, string eventKey = "")
    {
        node.speakerName = speaker;
        node.dialogueText = text;
        node.nextNode = next;
        node.choices = choices ?? new Choice[0];
        node.moneyDelta = moneyDelta;
        node.eventKey = eventKey;
    }

    static Choice C(string text, int money, DialogueNode next, string eventKey = "") =>
        new Choice { choiceText = text, moneyDelta = money, nextNode = next, eventKey = eventKey };
}
