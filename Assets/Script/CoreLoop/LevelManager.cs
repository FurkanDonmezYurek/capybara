using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject capybaraPrefab;
    public Color[] possibleColors;
    public int capybaraCount = 12;

    //for test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CapybaraSpawnerRandom();
        }
    }

    public void CapybaraSpawnerRandom()
    {
        var seats = new List<Seat>(FindObjectsOfType<Seat>());
        List<Seat> available = new List<Seat>(seats);

        for (int i = 0; i < capybaraCount && available.Count > 0; i++)
        {
            int index = Random.Range(0, available.Count);
            Seat seat = available[index];
            available.RemoveAt(index);

            var capy = Instantiate(capybaraPrefab, seat.transform.position, Quaternion.identity)
                .GetComponent<Capybara>();

            Color color = possibleColors[Random.Range(0, possibleColors.Length)];
            capy.SetColor(color);
            capy.SitSeat(seat);
        }
    }
}
