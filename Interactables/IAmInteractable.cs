using Bartox.Audio;

namespace Bartox
{
    public interface IAmInteractable
    {
        void Interact();
        void PlaySound(FMODEvent fEvent);
    }
}