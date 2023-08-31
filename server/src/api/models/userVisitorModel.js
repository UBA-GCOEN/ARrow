import mongoose from "mongoose";


//estimated visitor schema
const userVisitorModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     bio: { type: String, required: true },

})

export default mongoose.model("userVisitors", userVisitorModel);