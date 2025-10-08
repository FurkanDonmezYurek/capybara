using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public enum CapybaraType
{
    Normal,
    Fat,
    Child,
    Sleepy,
}

public class Capybara : MonoBehaviour
{
    public virtual CapybaraType Type => CapybaraType.Normal;
    public virtual float MoveSpeed => 5f;
    public Color color;
    public Seat currentSlot;
    protected bool isLocked;
    public bool isFrozen;
    public bool IsFrozen => isFrozen;
    public GameObject iceCubeVisual;
    public GameObject capybaraColorMaterialObject;
    public CapybaraStateMachine CapybaraStateMachine { get; private set; }
    private bool _isMoving;
    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            if (_isMoving == value)
                return; // Değer değişmemişse işlem yapma
            _isMoving = value;

            if (_isMoving)
            {
                if (!GameManager.Instance.movingCapybaras.Contains(this))
                {
                    GameManager.Instance.movingCapybaras.Add(this);
                }
            }
            else
            {
                GameManager.Instance.movingCapybaras.Remove(this);
            }
        }
    }

    private Renderer _capybaraRenderer;
    private int _accessoryMaterialIndex = -1;
    private static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");

    public virtual void Start()
    {
        if (capybaraColorMaterialObject != null)
        {
            _capybaraRenderer = capybaraColorMaterialObject.GetComponent<Renderer>();
            if (_capybaraRenderer != null)
            {
                var materials = _capybaraRenderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains("Capybara_Accesoires"))
                    {
                        _accessoryMaterialIndex = i;
                        break; // Materyali bulduk, döngüden çıkabiliriz.
                    }
                }
            }
        }
        CapybaraStateMachine = GetComponent<CapybaraStateMachine>();
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.FonksiyonStart();
        SitAnimation();
    }

    public virtual void SetSeat(Seat slot)
    {
        currentSlot = slot;
        slot.SetCapybara(this);
        transform.position = slot.transform.position;
        SitAnimation();
    }

    public virtual void SitAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        switch (Type)
        {
            case CapybaraType.Normal:
                CapybaraStateMachine.SetState(CapybaraStateMachine.normalSitState);
                break;
            case CapybaraType.Fat:
                CapybaraStateMachine.SetState(CapybaraStateMachine.fatSitState);
                break;
            case CapybaraType.Child:
                CapybaraStateMachine.SetState(CapybaraStateMachine.childSitState);
                break;
            case CapybaraType.Sleepy:
                CapybaraStateMachine.SetState(CapybaraStateMachine.sleepState);
                break;
        }

        ResetRotation();
    }

    public virtual void JumpAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.SetState(CapybaraStateMachine.jumpState);
    }

    public virtual void WalkAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.SetState(CapybaraStateMachine.walkState);
    }

    public virtual void RunAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.SetState(CapybaraStateMachine.runState);
    }

    public virtual void IdleAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.SetState(CapybaraStateMachine.idleState);
    }

    public virtual void FreezeAnimation()
    {
        if (CapybaraStateMachine == null)
        {
            return;
        }

        CapybaraStateMachine.SetState(CapybaraStateMachine.freezeState);
    }

    public void SetColor(Color newColor)
    {
        if (capybaraColorMaterialObject != null)
        {
            _capybaraRenderer = capybaraColorMaterialObject.GetComponent<Renderer>();
            if (_capybaraRenderer != null)
            {
                var materials = _capybaraRenderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains("Capybara_Accesoires"))
                    {
                        _accessoryMaterialIndex = i;
                        break; // Materyali bulduk, döngüden çıkabiliriz.
                    }
                }
            }
        }
        // Gerekli bileşenler bulunamadıysa veya materyal index'i geçersizse metottan çık.
        if (_capybaraRenderer == null || _accessoryMaterialIndex == -1)
        {
            return;
        }

        this.color = newColor;

        // 1. Yeni bir MaterialPropertyBlock oluştur.
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        // 2. Renderer'dan mevcut özellikleri bu bloğa kopyala (önemli!).
        // Bu sayede sadece rengi değiştirip diğer ayarları korumuş oluruz.
        _capybaraRenderer.GetPropertyBlock(propertyBlock, _accessoryMaterialIndex);

        // 3. Bloğun renk özelliğini ayarla.
        propertyBlock.SetColor(ColorPropertyID, newColor);

        // 4. Güncellenmiş bloğu sadece aksesuar materyali için renderer'a geri ata.
        _capybaraRenderer.SetPropertyBlock(propertyBlock, _accessoryMaterialIndex);
    }

    public virtual void Freeze()
    {
        isFrozen = true;
        if (iceCubeVisual != null)
            iceCubeVisual.SetActive(true);
        FreezeAnimation();
    }

    public virtual void Unfreeze()
    {
        isFrozen = false;
        if (iceCubeVisual != null)
            iceCubeVisual.SetActive(false);

        // Check the group color when unfrozen
        if (currentSlot != null)
        {
            currentSlot.groupOfSeat.CheckGroupColor();
        }
        SitAnimation();
    }

    public virtual bool IsMovable()
    {
        return !isLocked && !isFrozen;
    }

    public virtual void SitSeat(Seat targetSlot)
    {
        if (!CanSitTo(targetSlot))
        {
            return;
        }

        if (currentSlot == null)
        {
            DirectPlace(targetSlot);
            return;
        }

        if (IsSameGroup(currentSlot, targetSlot))
        {
            AnimateDirectMove(targetSlot);
        }
        else
        {
            AnimateCorridorMove(targetSlot);
        }
    }

    protected virtual bool CanSitTo(Seat targetSlot)
    {
        return IsMovable() && targetSlot != null;
    }

    protected virtual void DirectPlace(Seat slot)
    {
        slot.SetCapybara(this);
        currentSlot = slot;
        transform.position = slot.transform.position;
        SitAnimation();
    }

    protected virtual bool IsSameGroup(Seat a, Seat b)
    {
        return a.groupOfSeat == b.groupOfSeat;
    }

    protected void LookTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f; // Only rotate on horizontal axis
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.DORotateQuaternion(targetRotation, 0.3f); // Smooth rotate
        }
    }

    public virtual void ResetRotation()
    {
        transform.DORotateQuaternion(Quaternion.identity, 0.3f); // Smooth reset to default (forward: Z+)
    }

    protected virtual void AnimateDirectMove(Seat targetSlot)
    {
        IsMoving = true;

        WalkAnimation();

        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        Vector3 start = transform.position;
        Vector3 end = targetSlot.transform.position;

        LookTowards(end); // Look towards the target slot

        float duration = Vector3.Distance(start, end) / MoveSpeed;

        transform
            .DOMove(end, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                currentSlot = targetSlot;
                CheckTargetSeatMatch(targetSlot);
                SitAnimation();
                IsMoving = false;
            });
    }

    protected virtual void AnimateCorridorMove(Seat targetSlot)
    {
        WalkAnimation();

        currentSlot.ClearCapybara();
        targetSlot.SetCapybara(this);

        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            return;
        }

        Vector3 start = transform.position;
        Vector3 end = targetSlot.transform.position;

        SeatGroup fromGroup = currentSlot.groupOfSeat;
        SeatGroup toGroup = targetSlot.groupOfSeat;

        int fromX = fromGroup.groupX;
        int toX = toGroup.groupX;
        int y = fromGroup.groupY; // corridor satırı

        Vector3 fromExit = GetCorridorExitPoint(currentSlot, targetSlot);
        Vector3 toEntry = GetCorridorExitPoint(targetSlot, currentSlot);

        List<Vector3> pathPoints = new();
        pathPoints.Add(start); // A - Başlangıç noktası
        pathPoints.Add(fromExit); // B - Çıkış corridor noktası

        // corridor üzerindeki geçişler (gruplar arasında)
        if (fromX < toX)
        {
            for (int x = fromX; x < toX; x++)
            {
                pathPoints.Add(gridSystem.pathPointsGrid[x.ToString()][y.ToString()]);
            }
        }
        else if (fromX > toX)
        {
            for (int x = fromX - 1; x >= toX; x--)
            {
                pathPoints.Add(gridSystem.pathPointsGrid[x.ToString()][y.ToString()]);
            }
        }

        pathPoints.Add(toEntry); // C - Hedef grubun corridor giriş noktası
        pathPoints.Add(end); // D - Hedef koltuk

        IsMoving = true;

        // Animasyon
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 from = pathPoints[i];
            Vector3 to = pathPoints[i + 1];

            float dist = Vector3.Distance(from, to);
            float dur = dist / MoveSpeed;

            seq.AppendCallback(() => LookTowards(to));
            seq.Append(transform.DOMove(to, dur).SetEase(Ease.Linear));
        }

        seq.OnComplete(() =>
        {
            currentSlot = targetSlot;
            CheckTargetSeatMatch(targetSlot);
            SitAnimation();
            IsMoving = false;
        });
    }

    protected virtual Vector3 GetCorridorExitPoint(Seat seat, Seat targetSeat)
    {
        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null || gridSystem.pathPointsGrid == null)
            return seat.transform.position;

        int groupX = seat.groupOfSeat.groupX;
        int groupY = seat.groupOfSeat.groupY;

        List<Vector3> candidatePoints = new();

        // GridSystem'deki corridor noktaları arasında kendi grubunun sağındaki corridor da olabilir
        if (groupX > 0)
        {
            string leftKey = (groupX - 1).ToString();
            if (
                gridSystem.pathPointsGrid.TryGetValue(leftKey, out var yDict)
                && yDict.TryGetValue(groupY.ToString(), out var point)
            )
            {
                candidatePoints.Add(point);
            }
        }

        string currentKey = groupX.ToString();
        if (
            gridSystem.pathPointsGrid.TryGetValue(currentKey, out var currentYDict)
            && currentYDict.TryGetValue(groupY.ToString(), out var currentPoint)
        )
        {
            candidatePoints.Add(currentPoint);
        }

        // En yakın corridor noktasını hedefe göre seç
        Vector3 targetPos = targetSeat.transform.position;
        Vector3 bestPoint = candidatePoints
            .OrderBy(p => Vector3.Distance(p, targetPos))
            .FirstOrDefault();

        return new Vector3(bestPoint.x, seat.transform.position.y, seat.transform.position.z);
    }

    public void CheckTargetSeatMatch(Seat targetSlot)
    {
        var group = targetSlot.GetComponentInParent<SeatGroup>();
        group?.CheckGroupColor();
        if (Application.isPlaying)
            GameManager.Instance.CheckGameCondition();
    }

    public virtual void Lock()
    {
        isLocked = true;
    }

    public void ClickedCapybara()
    {
        if (isLocked || isFrozen || IsMoving)
            return;

        GameManager.Instance.OnCapybaraClicked(this);
    }
}
