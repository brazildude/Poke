export const serverConfig = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'https://api.example.com',
  
  // Add the authenticateUser method that AuthContext is trying to use
  authenticateUser: async (token: string) => {
    try {
      const response = await fetch(`${serverConfig.API_BASE_URL}/auth`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (!response.ok) {
        throw new Error('Failed to authenticate user with server');
      }
      
      return await response.json();
    } catch (error) {
      console.error('Authentication error:', error);
      throw error;
    }
  }
};