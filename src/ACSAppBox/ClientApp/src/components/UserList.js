import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'
import UserListEntry from './UserListEntry';

const UserList = (props) => {

  const [loading, setLoading] = useState(true);
  const [userList, setUserList] = useState(null);

  useEffect(() => {
    console.log('UserList: running effect');
    const loadUsers = async () => {
      console.log('called loadUsers');

      const token = await authService.getAccessToken();

      const getUsersResponse = await fetch('/api/users', {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const users = await getUsersResponse.json();

      setUserList(users);
      setLoading(false);
    }

    loadUsers();
  }, []);
    
  return (
    <>
      { loading 
        ? <p><em>Loading...</em></p>
        : <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>ACS Id</th>
                <th>Copy token to clipboard</th>
              </tr>
            </thead>
            <tbody>
              {userList.map(user =>
                <UserListEntry key={user.id} user={user} />
              )}
            </tbody>
          </table>
      }
    </>
  );
}

export default UserList;