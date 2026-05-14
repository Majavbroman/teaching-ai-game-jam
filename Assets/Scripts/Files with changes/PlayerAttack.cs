using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class PlayerAttack : MonoBehaviour
{
    private enum State
    {
        Default,
        Attacking
    }

    [SerializeField] private float _cooldownTime = 0.25f;
    private float _lastAttacked = 0;
    private AnimationController _animationController;
    private State _currentState = State.Default;

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
        if (_lastAttacked < _cooldownTime || _currentState == State.Attacking) return;

        _currentState = State.Attacking;
        _animationController.PlayAnimationClip(out float time, _attackAnimation);

        await System.Threading.Tasks.Task.Delay((int)(time * 1000));
        _currentState = State.Default;
        _lastAttacked = 0; 
        _animationController.PlayAnimationClip(_defaultAnimation);
    }

    //MOVED THE COLLISION DETECTION IN HERE FROM ENEMY SCRIPT TO FIX BUG
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Enemy enemy)) return;

        enemy.TryAttack();
    }
}
