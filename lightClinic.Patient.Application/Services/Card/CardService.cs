using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Card;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Card;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lightClinic.Patient.Application.Services.Card;

public class CardService : ICardService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCardDto> _createValidator;
    private readonly IValidator<UpdateCardDto> _updateValidator;

    public CardService(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMapper mapper,
        IValidator<CreateCardDto> createValidator,
        IValidator<UpdateCardDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IResult<CardDto>> GetAsync(long id, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .ProjectTo<CardDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (card is null)
        {
            return await Result<CardDto>.FailureAsync("Card not found!", System.Net.HttpStatusCode.BadRequest);
        }
        if (card.UserId != _currentUser.GetUserId())
        {
            return await Result<CardDto>.FailureAsync("Card does not belong to you!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<CardDto>.SuccessAsync(card);
    }

    public async Task<IResult<List<CardDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cards = await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .Where(x => x.UserId == _currentUser.GetUserId())
            .ProjectTo<CardDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<CardDto>>.SuccessAsync(cards);
    }

    public async Task<IResult<CardDto>> CreateAsync(CreateCardDto cardDto, CancellationToken cancellationToken)
    {
        var result = _createValidator.Validate(cardDto);
        if (!result.IsValid)
        {
            return await Result<CardDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }
        var validations = await CreateValidationAsync(cardDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<CardDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        var card = _mapper.Map<Domain.Entities.Card>(cardDto);
        card.UserId = _currentUser.GetUserId();

        await _unitOfWork.Repository<Domain.Entities.Card>().AddAsync(card);

        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<CardDto>.FailureAsync("Connection failed!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<CardDto>.SuccessAsync(_mapper.Map<CardDto>(card), "Card created successfully");
    }

    public async Task<IResult<CardDto>> UpdateAsync(UpdateCardDto cardDto, CancellationToken cancellationToken)
    {
        var result = _updateValidator.Validate(cardDto);
        if (!result.IsValid)
        {
            return await Result<CardDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var card = await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .FirstOrDefaultAsync(x => x.Id == cardDto.Id, cancellationToken);

        var validations = await UpdateValidationAsync(card, cardDto, cancellationToken);
        if (validations is not null)
        {
            return await Result<CardDto>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        _mapper.Map(cardDto, card);

        await _unitOfWork.Repository<Domain.Entities.Card>().UpdateAsync(card);
        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<CardDto>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<CardDto>.SuccessAsync(_mapper.Map<CardDto>(card), "Card updated successfully");
    }

    public async Task<IResult<bool>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return await Result<bool>.FailureAsync("Invalid Id", System.Net.HttpStatusCode.BadRequest);
        }
        var card = await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        var validations = DeleteValidation(card);
        if (validations is not null)
        {
            return await Result<bool>.FailureAsync(validations, System.Net.HttpStatusCode.BadRequest);
        }

        await _unitOfWork.Repository<Domain.Entities.Card>().DeleteAsync(card);
        if (await _unitOfWork.SaveAsync(cancellationToken) == 0)
        {
            return await Result<bool>.FailureAsync("Data unchanged!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<bool>.SuccessAsync("Card deleted successfully");
    }

    public async Task<IResult<bool>> ExistsAsync(long id, CancellationToken cancellationToken)
    {
        var valid = await _unitOfWork.Repository<Domain.Entities.Card>().Entities.AnyAsync(x => x.Id == id && x.UserId == _currentUser.GetUserId());

        return await Result<bool>.SuccessAsync(valid);
    }

    private async Task<string> CreateValidationAsync(CreateCardDto cardDto, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .AnyAsync(x
                => x.UserId == _currentUser.GetUserId()
                && x.CardNumber == cardDto.CardNumber
                && x.CardHolderName.Trim().ToLower() == cardDto.CardHolderName.Trim().ToLower()
                && x.CVVCode == cardDto.CVVCode, cancellationToken))
        {
            return "Card already exists";
        }

        return null;
    }

    private async Task<string> UpdateValidationAsync(Domain.Entities.Card card, UpdateCardDto cardDto, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        if (card is null)
        {
            return "Card not found!";
        }
        if (card.UserId != userId)
        {
            return "Card does not belong to you!";
        }
        if (await _unitOfWork.Repository<Domain.Entities.Card>().Entities
            .AnyAsync(x
                => x.Id != cardDto.Id
                && x.UserId == userId
                && x.CardNumber == cardDto.CardNumber
                && x.CardHolderName.Trim().ToLower() == cardDto.CardHolderName.Trim().ToLower()
                && x.CVVCode == cardDto.CVVCode, cancellationToken))
        {
            return "Card already exists";
        }

        return null;
    }

    private string DeleteValidation(Domain.Entities.Card card)
    {
        if (card is null)
        {
            return "Card not found!";
        }

        return null;
    }
}