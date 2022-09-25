import React from 'react';
import authService from './api-authorization/AuthorizeService'

const UserListEntry = (props) => {

  const { user } = props;

  const copyTokenToClipboard = async (id) => {
    const token = await authService.getAccessToken();

    // get ACS token
    const acsTokenResponse = await fetch(`/api/users/${id}/token`, {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const acsToken = await acsTokenResponse.json();

    console.log('got acs token', acsToken);
    navigator.clipboard.writeText(acsToken.token);
  }

  return (
    <tr>
      <td>{user.name}</td>
      <td>{user.email}</td>
      <td>{user.communicationUserId}</td>
      <td><button className="btn btn-primary" onClick={() => copyTokenToClipboard(user.id)}>Copy</button></td>
    </tr>
  );
}

export default UserListEntry;