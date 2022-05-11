import { useState } from 'react';

export default function UseToken() {
  const getToken = () => {
    const tokenString = localStorage.getItem('token');
    if (tokenString == null){
      return null
    }
    const userToken = JSON.parse(tokenString);
    return userToken?.token
  };

  const [token, setToken] = useState<string | null>(getToken());

  const saveToken = (userToken : any) => {
    localStorage.setItem('token', JSON.stringify(userToken));
    setToken(userToken?.token);
  };

  return {
    setToken: saveToken,
    token
  }
}