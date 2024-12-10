using Zentech.Models;

public class ContactService
{
    private readonly ContactRepository _repository;

    public ContactService(ContactRepository repository)
    {
        _repository = repository;
    }

    // Récupérer toutes les demandes de contact
    public async Task<List<ContactMessage>> GetAllContactsAsync()
    {
        return await _repository.GetAllAsync();
    }

    // Récupérer une demande de contact par ID
    public async Task<ContactMessage?> GetContactByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // Créer une nouvelle demande de contact
    public async Task<ContactMessage> CreateContactAsync(ContactMessage contactMessage)
    {
        return await _repository.AddAsync(contactMessage);
    }

    // Mettre à jour une demande de contact existante
    public async Task<ContactMessage?> UpdateContactAsync(ContactMessage contactMessage)
    {
        return await _repository.UpdateAsync(contactMessage);
    }

    // Supprimer une demande de contact
    public async Task<bool> DeleteContactAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
