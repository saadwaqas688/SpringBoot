# Frontend React Application

## Running the Application

```bash
npm install
npm start
```

The app will start on `http://localhost:3000`

## Environment Variables

Create a `.env` file in the frontend directory:

```env
REACT_APP_API_URL=http://localhost:5000/api
```

If not set, it defaults to `http://localhost:5000/api`

## Features

- Modern WhatsApp-like UI
- Real-time messaging with SignalR
- Responsive design
- Media upload and display
- Message reactions and replies
- Online status indicators
- Group chat management

## Project Structure

```
src/
├── components/
│   ├── Auth/          # Login and Register components
│   └── Chat/          # Chat-related components
├── contexts/          # React contexts (Auth)
├── services/          # API and SignalR services
└── App.js             # Main application component
```

