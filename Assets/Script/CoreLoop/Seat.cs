using System.Collections;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Capybara currentCapybara;
    public bool IsEmpty => currentCapybara == null;
    public bool isCorridorSide;
    public SeatGroup groupOfSeat;

    [SerializeField]
    private GameObject seatMeshObj;

    // Sets current capybara
    public void SetCapybara(Capybara capy)
    {
        currentCapybara = capy;
    }

    public void ClearCapybara()
    {
        currentCapybara = null;
    }

    public bool IsOccupiedBy(Capybara capy)
    {
        return currentCapybara == capy;
    }

    private void OnMouseDown()
    {
        if (currentCapybara == null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSeatClicked(this);
        }
        else
        {
            if (
                UIManager.Instance.levelCompletePanel.activeSelf
                || GameTimerManager.Instance.currentTime <= 0
            )
                return;
            currentCapybara.ClickedCapybara();
        }
    }

    public void GlowSeat(bool isGreen)
    {
        if (isGreen)
        {
            seatMeshObj.layer = LayerMask.NameToLayer("GreenGlow");
        }
        else
        {
            seatMeshObj.layer = LayerMask.NameToLayer("RedGlow");
        }

        StartCoroutine(ResetLayer(!isGreen, 0.3f));
    }

    IEnumerator ResetLayer(bool rightNow, float time)
    {
        if (rightNow)
            yield return new WaitForSeconds(time);
        else
            yield return new WaitUntil(() =>
                currentCapybara != null && currentCapybara.IsMoving == false
            );

        seatMeshObj.layer = LayerMask.NameToLayer("Default");
    }
}
