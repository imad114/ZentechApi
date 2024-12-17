using Zentech.Models;

public class ContactService
{
    private readonly ContactRepository _repository;

    public ContactService(ContactRepository repository)
    {
        _repository = repository;
    }

 
    public async Task<List<ContactMessage>> GetAllContactsAsync()
    {
        return await _repository.GetAllAsync();
    }

    
    public async Task<ContactMessage?> GetContactByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

   
    public async Task<ContactMessage> CreateContactAsync(ContactMessage contactMessage)
    {
        return await _repository.AddAsync(contactMessage);
    }

  
    public async Task<ContactMessage?> UpdateContactAsync(ContactMessage contactMessage)
    {
        return await _repository.UpdateAsync(contactMessage);
    }

  
    public async Task<bool> DeleteContactAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
