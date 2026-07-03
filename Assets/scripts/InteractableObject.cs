using UnityEngine;

public enum ObjectType
{
    Tree,
    Rock,
    WildPlant
}

public class InteractableObject : MonoBehaviour
{
    [Header("Pengaturan Objek")]
    public ObjectType objectType;
    public int health = 3;

    // Fungsi ini dipanggil oleh PlayerController saat objek ini terdeteksi serangan
    public void OnHit(ItemType toolUsed)
    {
        bool isToolMatching = false;

        // Mengecek apakah alat yang digunakan cocok dengan jenis objek
        switch (objectType)
        {
            case ObjectType.Tree:
                if (toolUsed == ItemType.Axe) isToolMatching = true;
                break;
            case ObjectType.Rock:
                if (toolUsed == ItemType.Pickaxe) isToolMatching = true;
                break;
            case ObjectType.WildPlant:
                if (toolUsed == ItemType.BareHanded) isToolMatching = true;
                break;
        }

        if (isToolMatching)
        {
            health--;
            Debug.Log($"{gameObject.name} terkena hit! Sisa darah: {health}");

            // Tambahkan efek getaran / animasi 'Hit' pada objek di sini jika ada

            if (health <= 0)
            {
                DestroyObject();
            }
        }
        else
        {
            Debug.Log($"Alat tidak cocok! Anda butuh alat yang tepat untuk {gameObject.name}");
        }
    }

    private void DestroyObject()
    {
        Debug.Log($"{gameObject.name} hancur/terpanen!");
        
        // TODO: Anda bisa menambahkan logika instantiate/drop item resource (kayu/batu) di sini
        
        Destroy(gameObject);
    }
}
