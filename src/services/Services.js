import axios from 'axios';
const getBookingDetailsURL = 'https://localhost:44396/api/ChatbotBell/GetBookingDetails';

export const getAllBookingRequestMethod= async ()=>{
    const response = await axios.get(getBookingDetailsURL);
    return response;
}