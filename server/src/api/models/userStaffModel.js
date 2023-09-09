import mongoose from "mongoose";


//estimated staff schema
const userStaffModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     // branch: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     // designation: { type: String, required: true },
     role: { type: String, default: 'staff'},
     bio: { type: String },
     mobile: { type: Number }

})

export default mongoose.model("userStaffs",userStaffModel);