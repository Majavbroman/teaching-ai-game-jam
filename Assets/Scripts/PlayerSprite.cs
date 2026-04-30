using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class PlayerSprite : MonoBehaviour
{
    [SerializeField] private AnimationClip _idleAnimation;
    [SerializeField] private AnimationClip _runAnimation;

    private AnimationController _animationController;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();

        if (transform.root.TryGetComponent(out PlayerController controller))
        {
            controller.MoveEvent += SetWalking;
        }
    }

    private void SetWalking(Vector2 direction)
    {
        if (direction.magnitude == 0)
        {
            _animationController.PlayAnimationClip(_idleAnimation);
            return;
        }
        
        _animationController.PlayAnimationClip(_runAnimation);
    }
}
