import mongoose from "mongoose";


//estimated user schema
const userGuestModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     bio: { type: String, required: true },

})

export default mongoose.model("userGuests", userGuestModel);