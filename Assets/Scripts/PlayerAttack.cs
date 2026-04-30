using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float _cooldownTime = 0.25f;
    private float _lastAttacked = 0;
    private AnimationController _animationController;

    [SerializeField] private AnimationClip _attackAnimation;
    [SerializeField] private AnimationClip _defaultAnimation;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();

        if (transform.root.TryGetComponent(out PlayerController controller))
        {
            controller.AttackEvent += Attack;
        }
    }

    private void Update()
    {
        _lastAttacked += Time.deltaTime;
    }

    private async void Attack()
    {
        if (_lastAttacked < _cooldownTime) return;

        _lastAttacked = 0;
        _animationController.PlayAnimationClip(out float time, _attackAnimation);

        await System.Threading.Tasks.Task.Delay((int)(time * 1000));
        _lastAttacked = 0; 
        _animationController.PlayAnimationClip(_defaultAnimation);
    }
}
