using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour

{
    [SerializeField] private LevelLockManager levelLockManager;  // Level kilitlerini yöneten script
    private int currentLevel = 1;

    // Bu fonksiyon her level tamamlandığında çağrılacak
    public void CompleteLevel()
    {
        // Örnek olarak Level 1 tamamlandığında Level 2'nin kilidini açıyoruz
        if (currentLevel == 1)
        {
            UnlockNextLevel();
        }
    }

    // Bir sonraki seviyenin kilidini aç
    private void UnlockNextLevel()
    {
        // Burada currentLevel'ı artırıyoruz
        currentLevel++;
        levelLockManager.UnlockNextLevel();  // LevelLockManager'dan kilidi açma fonksiyonunu çağırıyoruz
    }

}
