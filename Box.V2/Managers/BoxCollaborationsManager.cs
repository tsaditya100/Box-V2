﻿using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Converter;
using Box.V2.Extensions;
using Box.V2.Models;
using Box.V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Box.V2.Managers
{
    public class BoxCollaborationsManager : BoxResourceManager
    {

        public BoxCollaborationsManager(IBoxConfig config, IBoxService service, IBoxConverter converter, IAuthRepository auth, string asUser = null, bool? suppressNotifications = null)
            : base(config, service, converter, auth, asUser, suppressNotifications) { }

        /// <summary>
        /// Used to add a collaboration for a single user to a folder. Descriptions of the various roles can be found 
        /// <see cref="https://support.box.com/entries/20366031-what-are-the-different-collaboration-permissions-and-what-access-do-they-provide"/>
        /// Either an email address or a user ID can be used to create the collaboration.
        /// </summary>
        /// <param name="collaborationRequest"></param>
        /// <param name="fields">Attribute(s) to include in the response</param>
        /// <param name="notify">Determines if the user, (or all the users in the group) should receive email notification of the collaboration.</param>
        /// <returns></returns>
        public async Task<BoxCollaboration> AddCollaborationAsync(BoxCollaborationRequest collaborationRequest, List<string> fields = null, bool? notify = null)
        {
            collaborationRequest.ThrowIfNull("collaborationRequest")
                .Item.ThrowIfNull("collaborationRequest.Item")
                .Id.ThrowIfNullOrWhiteSpace("collaborationRequest.Item.Id");
            collaborationRequest.AccessibleBy.ThrowIfNull("collaborationRequest.AccessibleBy");

            BoxRequest request = new BoxRequest(_config.CollaborationsEndpointUri)
                .Method(RequestMethod.Post)
                .Param(ParamFields, fields)
                .Payload(_converter.Serialize(collaborationRequest));

            if (notify.HasValue)
            {
                var value = notify.Value ? "true" : "false";
                request.Param("notify", value);
            }

            IBoxResponse<BoxCollaboration> response = await ToResponseAsync<BoxCollaboration>(request).ConfigureAwait(false);

            return response.ResponseObject;
        }

        /// <summary>
        /// Used to edit an existing collaboration. Descriptions of the various roles can be found 
        /// <see cref="https://support.box.com/entries/20366031-what-are-the-different-collaboration-permissions-and-what-access-do-they-provide"/>
        /// </summary>
        /// <param name="collaborationRequest"></param>
        /// <returns></returns>
        public async Task<BoxCollaboration> EditCollaborationAsync(BoxCollaborationRequest collaborationRequest, List<string> fields = null)
        {
            collaborationRequest.ThrowIfNull("collaborationRequest")
                .Id.ThrowIfNullOrWhiteSpace("collaborationRequest.Id");

            BoxRequest request = new BoxRequest(_config.CollaborationsEndpointUri, collaborationRequest.Id)
                .Method(RequestMethod.Put)
                .Param(ParamFields, fields)
                .Payload(_converter.Serialize(collaborationRequest));

            IBoxResponse<BoxCollaboration> response = await ToResponseAsync<BoxCollaboration>(request).ConfigureAwait(false);

            return response.ResponseObject;
        }

        /// <summary>
        /// Used to delete a single collaboration.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCollaborationAsync(string id)
        {
            id.ThrowIfNullOrWhiteSpace("id");

            BoxRequest request = new BoxRequest(_config.CollaborationsEndpointUri, id)
                .Method(RequestMethod.Delete);

            IBoxResponse<BoxCollaboration> response = await ToResponseAsync<BoxCollaboration>(request).ConfigureAwait(false);

            return response.Status == ResponseStatus.Success;
        }

        /// <summary>
        /// /// Used to get information about a single collaboration. A complete list of the user’s pending collaborations can also be retrieved.
        /// <see cref="http://developers.box.com/docs/#collaborations-get-pending-collaborations"/>
        /// </summary>
        /// <param name="collaborationRequest"></param>
        /// <returns></returns>
        public async Task<BoxCollaboration> GetCollaborationAsync(string id, List<string> fields = null)
        {
            id.ThrowIfNullOrWhiteSpace("id");

            BoxRequest request = new BoxRequest(_config.CollaborationsEndpointUri, id)
                .Param(ParamFields, fields);

            IBoxResponse<BoxCollaboration> response = await ToResponseAsync<BoxCollaboration>(request).ConfigureAwait(false);

            return response.ResponseObject;
        }
        /// <summary>
        /// Used to retrieve all pending collaboration invites for this user.
        /// </summary>
        /// <param name="fields">Attribute(s) to include in the response</param>
        /// <returns>A collection of pending collaboration objects are returned. If the user has no pending collaborations, the collection will be empty.</returns>
        public async Task<BoxCollection<BoxCollaboration>> GetPendingCollaborationAsync(List<string> fields = null)
        {
           
            BoxRequest request = new BoxRequest(_config.CollaborationsEndpointUri, null)
               .Param(Constants.RequestParameters.Status, Constants.RequestParameters.Pending)
               .Param(ParamFields, fields);

            IBoxResponse<BoxCollection<BoxCollaboration>> response = await ToResponseAsync<BoxCollection<BoxCollaboration>>(request).ConfigureAwait(false);
            return response.ResponseObject;
        }
    }
}
