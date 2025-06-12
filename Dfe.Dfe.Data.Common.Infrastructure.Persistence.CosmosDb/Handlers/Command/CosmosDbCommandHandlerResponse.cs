using Microsoft.Azure.Cosmos;

namespace Dfe.Data.Common.Infrastructure.Persistence.CosmosDb.Handlers.Command;

/// <summary>
/// Encapsulates the response of a Cosmos DB command, providing
/// meta-data about success, the returned item, and potential errors.
/// </summary>
public sealed class CosmosDbCommandHandlerResponse<TItem>
{
    // Stores the response from Cosmos DB operations.
    private readonly ItemResponse<TItem> _commandResponse;

    /// <summary>
    /// Constructor to initialize the response wrapper with a valid response object.
    /// Throws an exception if the provided response is null.
    /// </summary>
    /// <param name="commandResponse">The response object from Cosmos DB.</param>
    public CosmosDbCommandHandlerResponse(ItemResponse<TItem> commandResponse)
    {
        _commandResponse = commandResponse ??
            throw new ArgumentNullException(nameof(commandResponse));
    }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// Success is determined if the status code falls within the 200–299 range.
    /// </summary>
    public bool Success =>
        _commandResponse.StatusCode
            is >= System.Net.HttpStatusCode.OK          // StatusCode >= 200.
            and < System.Net.HttpStatusCode.Ambiguous;  // StatusCode < 300.

    /// <summary>
    /// Retrieves the resource (item) from the response, if available.
    /// </summary>
    public TItem? Item => _commandResponse.Resource; // Extracts the returned item.

    /// <summary>
    /// If the operation was not successful, returns a CosmosException containing details.
    /// Otherwise, returns null.
    /// </summary>
    public Exception? Error =>
        _commandResponse.StatusCode != System.Net.HttpStatusCode.OK ?
            new CosmosException(
                "Unable to complete requested command.",    // Custom error message.
                _commandResponse.StatusCode,                // Response status code.
                (int)_commandResponse.StatusCode,           // Numeric status code.
                _commandResponse.ActivityId,                // Request activity identifier.
                _commandResponse.RequestCharge)             // Operation cost in request units (RU).
            : null!; // No error if operation was successful.

    /// <summary>
    /// Creates an instance of <see cref="CosmosDbCommandHandlerResponse{TItem}"/>
    /// using the provided Cosmos DB response. Ensures the response is not null.
    /// </summary>
    /// <param name="commandResponse">
    /// The response object from Cosmos DB.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="CosmosDbCommandHandlerResponse{TItem}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="commandResponse"/> is null.
    /// </exception>
    public static CosmosDbCommandHandlerResponse<TItem> Create(ItemResponse<TItem> commandResponse)
    {
        ArgumentNullException.ThrowIfNull(commandResponse);
        return new(commandResponse);
    }
}
