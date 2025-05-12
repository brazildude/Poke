export const serverConfig = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'https://api.example.com',
  
  // Add the authenticateUser method that AuthContext is trying to use
  authenticateUser: async (token: string) => {
    try {
      const response = await fetch(`${serverConfig.API_BASE_URL}/users`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({"Provider": "Google", "Token": token})
      });
      
      if (!response.ok) {
        throw new Error('Failed to authenticate user with server');
      }
      
      return true;
    } catch (error) {
      console.error('Authentication error:', error);
      throw error;
    }
  }
};